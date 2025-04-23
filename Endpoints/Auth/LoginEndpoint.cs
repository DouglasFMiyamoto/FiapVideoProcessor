using FastEndpoints;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FiapVideoProcessor.Endpoints.Auth
{
    public class LoginRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = default!;
    }

    public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
    {
        public IConfiguration Configuration { get; set; }

        public override void Configure()
        {
            Post("/auth/login");
            AllowAnonymous();
            Summary(s =>
            {
                s.Summary = "Realiza login e retorna o JWT";
            });
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            // Validação fake (você pode trocar por consulta ao banco depois)
            if (req.Username != "admin" || req.Password != "123")
            {
                await SendUnauthorizedAsync(ct);
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, req.Username),
                new Claim(ClaimTypes.NameIdentifier, "1")
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            await SendAsync(new LoginResponse
            {
                Token = tokenHandler.WriteToken(token)
            });
        }
    }
}
