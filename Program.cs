using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FiapVideoProcessor.Context;
using FiapVideoProcessor.Services;
using FiapVideoProcessor.Repositories;
using Amazon.SQS;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon;
using FiapVideoProcessor.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Configuração do PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var key = Encoding.ASCII.GetBytes(jwtKey!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.SaveToken = true;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

// FastEndpoints
builder.Services.AddFastEndpoints()
                .SwaggerDocument();

// Serviços da aplicação
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddScoped<IQueueService, QueueService>();
builder.Services.AddSingleton<SqsQueueInitializer>();

builder.Services.AddDefaultAWSOptions(new AWSOptions
{
    Credentials = new BasicAWSCredentials(
        builder.Configuration["AWS:AccessKey"],
        builder.Configuration["AWS:SecretKey"]
    ),
    Region = RegionEndpoint.USEast1,
    DefaultClientConfig =
    {
        ServiceURL = builder.Configuration["AWS:ServiceURL"]
    }
}); ;

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Users.Any())
    {
        db.Users.Add(new User { Id = 1, Username = "admin", Password = "123" });
        db.SaveChanges();
    }

    var queueInit = scope.ServiceProvider.GetRequiredService<SqsQueueInitializer>();
    await queueInit.InitializeAsync();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();