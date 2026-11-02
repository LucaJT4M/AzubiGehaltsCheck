using AzubiGehaltsCheck.Infrastructure.Identity;

namespace AzubiGehaltsCheck.Infrastructure.Security
{
    public interface IJwtTokenService
    {
        Task<(string Token, DateTimeOffset ExpiresAtUtc)> CreateAccessTokenAsync(ApplicationUser user);
    }
}