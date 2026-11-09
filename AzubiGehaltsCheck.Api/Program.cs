using System.Security.Claims;
using AzubiGehaltsCheck.Infrastructure;
using AzubiGehaltsCheck.Infrastructure.Data;
using AzubiGehaltsCheck.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddSwaggerGen();

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
//     .MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.Run();