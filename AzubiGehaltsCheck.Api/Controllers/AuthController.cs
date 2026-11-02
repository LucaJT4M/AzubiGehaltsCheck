using System.Security.Claims;
using AzubiGehaltsCheck.Api.DTOs.Auth;
using AzubiGehaltsCheck.Infrastructure.Identity;
using AzubiGehaltsCheck.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AzubiGehaltsCheck.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenService jwtTokenService) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

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

            var (token, expiresAtUtc) = await _jwtTokenService.CreateAccessTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                accessToken = token,
                accessTokenExpiresAt = expiresAtUtc,
                email = user.Email,
                roles
            });
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

            var (Token, ExpiresAtUtc) = await _jwtTokenService.CreateAccessTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                accessToken = Token,
                accessTokenExpiresAt = ExpiresAtUtc,
                email = user.Email,
                roles
            });
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
    }
}