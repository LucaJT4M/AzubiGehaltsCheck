using System.Text;
using AzubiGehaltsCheck.Infrastructure.Data;
using AzubiGehaltsCheck.Infrastructure.Identity;
using AzubiGehaltsCheck.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AzubiGehaltsCheck.Infrastructure
{
    public static class DependencyInjection
    {
        private static JwtOptions _getJwtOptions(IConfiguration configuration)
        {
            // Reads Jwt Settings from appsettings.json and throw error when missing
            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? throw new InvalidOperationException("Jwt configuration is missing.");

            // throw error if something is missing in the jwt settings
            if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
                string.IsNullOrWhiteSpace(jwtOptions.Audience) ||
                string.IsNullOrWhiteSpace(jwtOptions.Key))
            {
                throw new InvalidOperationException("Jwt settings are incomplete (Issuer/Audience/Key).");
            }

            return jwtOptions;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // PostgreSQL
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("appdb"));
            });

            // Identity
            services
                .AddIdentityApiEndpoints<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Jwt Tokens
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            var jwtOptions = _getJwtOptions(configuration);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                    };
                });

            // Authorization
            services.AddAuthorization();

            return services;
        }


    }
}