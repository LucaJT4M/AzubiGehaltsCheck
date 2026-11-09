using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AzubiGehaltsCheck.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AzubiGehaltsCheck.Api.Controllers;

[ApiController]
[Route("/api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly byte[] _signingKeyBytes;

    public AuthController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;

        var jwtSettings = configuration.GetSection("Authentication:Jwt");
        _issuer = jwtSettings["Issuer"]
            ?? throw new InvalidOperationException("JWT issuer is not configured.");
        _audience = jwtSettings["Audience"]
            ?? throw new InvalidOperationException("JWT audience is not configured.");

        var signingKeyBase64 = jwtSettings["SigningKey"]
            ?? throw new InvalidOperationException("JWT signing key is not configured.");

        _signingKeyBytes = Convert.FromBase64String(signingKeyBase64);
        if (_signingKeyBytes.Length < 32)
            throw new InvalidOperationException("JWT signing key must be at least 32 bytes.");
    }

    [AllowAnonymous]
    [HttpPost("jwt/login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!result.Succeeded)
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(15);

        // creating user content for jwt token
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // creating the token with expiry time
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(_signingKeyBytes),
                SecurityAlgorithms.HmacSha256));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new LoginResponse(accessToken, expiresAt));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("jwt/me")]
    public IActionResult GetCurrentUser()
    {
        return Ok(new { name = User.Identity?.Name });
    }
}

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = "";

    [Required]
    public string Password { get; init; } = "";
}

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);