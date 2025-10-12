using Application.Interfaces;
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {

        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;
        private readonly IRefreshTokenRepository _refreshTokenRepository;


        public JwtTokenService(IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            _issuer = _configuration["Jwt:Issuer"] ?? "EducationalPlatform";
            _audience = _configuration["Jwt:Audience"] ?? "EducationalPlatformUsers";
            _expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "1440"); // Default 24 hours
            _refreshTokenRepository = refreshTokenRepository;
        }

        public string GenerateToken(Guid userId, string email, string role, string fullName)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<string> GenerateRefreshToken(Guid UserId, CancellationToken cancellationToken)
        {
            var NewRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            await _refreshTokenRepository.AddRefreshTokenAsync(NewRefreshToken, UserId, cancellationToken);
            return NewRefreshToken;
        }
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
