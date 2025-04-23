using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using Amazon.Runtime;
using Amazon;
using FiapVideoProcessor.Context;
using FiapVideoProcessorWorker.Services;
using FiapVideoProcessor;

namespace FiapVideoProcessorWorker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    // PostgreSQL
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(configuration.GetConnectionString("Postgres")));

                    // AWS SQS
                    services.AddAWSService<IAmazonSQS>();
                    services.AddDefaultAWSOptions(new AWSOptions
                    {
                        Credentials = new BasicAWSCredentials(
                            configuration["AWS:AccessKey"],
                            configuration["AWS:SecretKey"]
                        ),
                        Region = RegionEndpoint.USEast1,
                        DefaultClientConfig =
                        {
                            ServiceURL = configuration["AWS:ServiceURL"]
                        }
                    });

                    // Serviço do Worker
                    services.AddHostedService<VideoProcessorService>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}