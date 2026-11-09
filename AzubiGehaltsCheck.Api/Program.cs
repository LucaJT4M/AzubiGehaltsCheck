using System.Security.Claims;
using AzubiGehaltsCheck.Infrastructure;
using AzubiGehaltsCheck.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

// config for the secret auth key stored in user secrets (when local dev)
var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("Authentication:Jwt");
var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");
var signingKeyBase64 = jwtSettings["SigningKey"] ?? throw new InvalidOperationException("JWT signing key is not configured.");

var signingKeyBytes = Convert.FromBase64String(signingKeyBase64);
if (signingKeyBytes.Length < 32)
{
    throw new InvalidOperationException("JWT signing key must be at least 32 bytes.");
}

builder.Services
    .AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(static options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste only the JWT token"
    });

    options.AddSecurityRequirement(static document =>
    {
        var bearerReference = new OpenApiSecuritySchemeReference(
            referenceId: "Bearer",
            hostDocument: document,
            externalResource: null);

        return new OpenApiSecurityRequirement
        {
            { bearerReference, new List<string>() }
        };
    });
});

// Infrastructure (PostgreSQL + Identity)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// this makes sure the db migrates automatically so you dont have to do it manual every time you update
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await db.Database.MigrateAsync();
}

// Aspire health endpoints
app.MapDefaultEndpoints();
await EnsureRolesAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// app.MapGroup("/auth")
// .MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.Run();

static async Task EnsureRolesAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "User", "Admin" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(static e => e.Description))}");
            }
        }
    }
}