using System.Security.Claims;
using AzubiGehaltsCheck.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AzubiGehaltsCheck.Infrastructure.Security
{
    public class JwtTokenService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions) : IJwtTokenService
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<(string Token, DateTimeOffset ExpiresAtUtc)> CreateAccessTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expires.UtcDateTime,
                signingCredentials: creds);

            var serialized = new JwtSecurityTokenHandler().WriteToken(token);
            return (serialized, expires);
        }
    }
}