using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AzubiGehaltsCheck.Api.DTOs.Auth;
using AzubiGehaltsCheck.Infrastructure.Data;
using AzubiGehaltsCheck.Infrastructure.Identity;
using AzubiGehaltsCheck.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzubiGehaltsCheck.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenService jwtTokenService, ApplicationDbContext dbContext) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        // vars for refresh Token config
        private const string RefreshTokenCookieName = "refreshToken";
        private const string RefreshTokenProvider = "AzubiGehaltsCheck";
        private const string RefreshTokenHashName = "RefreshTokenHash";
        private const string RefreshTokenExpiryName = "RefreshTokenExpiryUtc";

        [HttpPost("/register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Email already in use.",
                    Detail = "A user with this email already exists.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Description);
                return BadRequest(new ProblemDetails
                {
                    Title = "Registration failed.",
                    Detail = string.Join(" ", errors),
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // default role from your project plan
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Role assignment failed.",
                    Detail = string.Join(" ", errors),
                    Status = StatusCodes.Status500InternalServerError
                });
            }

            return await IssueTokensAsync(user);
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            // Do not reveal whether email exists (generic message)
            if (user is null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid credentials.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                request.Password,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return StatusCode(StatusCodes.Status423Locked, new ProblemDetails
                {
                    Title = "Account locked.",
                    Detail = "Too many failed attempts. Try again later.",
                    Status = StatusCodes.Status423Locked
                });
            }

            if (!result.Succeeded)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid credentials.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            return await IssueTokensAsync(user);
        }

        [Authorize]
        [HttpGet("/me")]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            return Ok(new
            {
                userId,
                email,
                roles
            });
        }

        [HttpPost("/refresh")]
        public async Task<IActionResult> Refresh()
        {
            // get cookie refresh token
            if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshTokenPlain) || string.IsNullOrWhiteSpace(refreshTokenPlain))
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Refresh token missing.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            // Find user by stored token hash
            var refreshTokenHash = ComputeHash(refreshTokenPlain);

            var tokenRow = await _dbContext.Set<IdentityUserToken<string>>()
                .AsNoTracking()
                .SingleOrDefaultAsync(t =>
                    t.LoginProvider == RefreshTokenProvider &&
                    t.Name == RefreshTokenHashName &&
                    t.Value == refreshTokenHash);

            if (tokenRow is null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid refresh token.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            // load user
            var user = await _userManager.FindByIdAsync(tokenRow.UserId);
            if (user is null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid refresh token.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            // validate that token is expired
            var expiryRaw = await _userManager.GetAuthenticationTokenAsync(user, RefreshTokenProvider, RefreshTokenExpiryName);

            if (!DateTimeOffset.TryParse(expiryRaw, out var expiresAt) || expiresAt <= DateTimeOffset.UtcNow)
            {
                await ClearRefreshTokenAsync(user); // cleanup old cookie + store
                return Unauthorized(new ProblemDetails
                {
                    Title = "Refresh token expired.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            return await IssueTokensAsync(user);
        }

        [Authorize]
        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = _userManager.GetUserId(User); // User var is stored in claim (you have to make sure that authorize is at the endpoint so that you get the user)
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "User is not authenticated.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "User is not authenticated.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            await ClearRefreshTokenAsync(user);

            return NoContent();
        }

        private static string ComputeHash(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
        private async Task ClearRefreshTokenAsync(ApplicationUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(
                user, RefreshTokenProvider, RefreshTokenHashName);

            await _userManager.RemoveAuthenticationTokenAsync(
                user, RefreshTokenProvider, RefreshTokenExpiryName);

            Response.Cookies.Delete(
                RefreshTokenCookieName,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
        }
        private async Task<IActionResult> IssueTokensAsync(ApplicationUser user)
        {
            // create access token, create refresh token and hash it
            var (accessToken, accessTokenExpiresAt) = await _jwtTokenService.CreateAccessTokenAsync(user);
            var refreshTokenPlain = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenHash = ComputeHash(refreshTokenPlain); // hash refresh Token
            var refreshExpiresAt = DateTimeOffset.UtcNow.AddDays(7);

            await _userManager.SetAuthenticationTokenAsync(user, RefreshTokenProvider, RefreshTokenHashName, refreshTokenHash);
            await _userManager.SetAuthenticationTokenAsync(user, RefreshTokenProvider, RefreshTokenExpiryName, refreshExpiresAt.ToString("O"));

            // Send plaintext refresh token as secure HttpOnly cookie
            Response.Cookies.Append(RefreshTokenCookieName, refreshTokenPlain, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,                 // requires HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = refreshExpiresAt,
                IsEssential = true
            });

            // Return access token payload
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                accessToken,
                accessTokenExpiresAt,
                email = user.Email,
                roles
            });
        }
    }
}