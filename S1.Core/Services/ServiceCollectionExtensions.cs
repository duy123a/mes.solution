using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using S1.Core.Configurations;
using S1.Core.Data.Context;
using S1.Core.Entities;
using System.Security.Claims;
using System.Text;

namespace S1.Core.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddErrorDescriber<AppErrorDescriber>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(10);
            });

            services.UseCookieAuthentication(configuration);

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });

            services.AddRepositories();

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services;
        }

        /// <summary>
        /// Only for JWT authentication (Web API)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection UseJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                  ?? throw new InvalidOperationException("JwtSettings section is missing.");
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
                options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
            });

            return services;
        }

        /// <summary>
        /// Only for cookie authentication (MVC)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection UseCookieAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var cookieSettings = configuration.GetSection("CookieSettings").Get<CookieSettings>()
                     ?? throw new InvalidOperationException("CookieSettings section is missing.");
            services.Configure<CookieSettings>(configuration.GetSection("CookieSettings"));

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = cookieSettings.LoginPath;
                options.LogoutPath = cookieSettings.LogoutPath;
                options.ExpireTimeSpan = TimeSpan.FromSeconds(cookieSettings.DefaultExpireSeconds);
                options.SlidingExpiration = cookieSettings.SlidingExpiration;
            });

            return services;
        }
    }
}
