using AddressBook.Application.Interfaces;
using AddressBook.Domain.Entites;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AddressBook.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private const int DefaultDurationInMinutes = 60;

        // HS256 requires a key larger than 256 bits.
        private const int MinimumKeyBytes = 33;

        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime ExpiresAt) CreateToken(AppUser user)
        {
            var section = _configuration.GetSection("JWT");

            var key = section["Key"];
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("JWT:Key is not configured.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);
            if (keyBytes.Length < MinimumKeyBytes)
            {
                // Signing would otherwise fail deep inside the crypto provider with an opaque IDX10720.
                throw new InvalidOperationException(
                    $"JWT:Key must be at least {MinimumKeyBytes} bytes for HS256; the configured value is {keyBytes.Length}.");
            }

            var issuer = section["Issuer"];
            var audience = section["Audience"];

            if (!int.TryParse(section["DurationInMinutes"], out var durationInMinutes) || durationInMinutes <= 0)
            {
                durationInMinutes = DefaultDurationInMinutes;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var signingKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(durationInMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}
