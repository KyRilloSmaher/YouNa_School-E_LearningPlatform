using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Application.ClockandUserContext;
using SharedKernel.Application.ClockandUserContext;
using SharedKernel.Application.UNIT_OF_WORK;
using SharedKernel.Infrastructure.Messaging.RabbitMQ;
using System.Text;
using YounaSchool.Authuntication.Application.Abstractions.Messaging;
using YounaSchool.Authuntication.Application.Abstractions.Persistence;
using YounaSchool.Authuntication.Application.Abstractions.Security;
using YounaSchool.Authuntication.Domain.Interfaces.Repositories;
using YounaSchool.Authuntication.Infrastructure.Identity;
using YounaSchool.Authuntication.Infrastructure.Messaging;
using YounaSchool.Authuntication.Infrastructure.Persistence;
using YounaSchool.Authuntication.Infrastructure.Persistence.Repositories;
using YounaSchool.Authuntication.Infrastructure.Security;

namespace YounaSchool.Authuntication.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Db")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        services.AddSingleton<ISystemClock, SystemClock>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        services.AddScoped<IUserCredentialRepository, UserCredentialRepository>();
        services.AddScoped<IAuthSessionRepository, AuthSessionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();
        services.AddHttpContextAccessor();
        services.AddScoped<SharedKernel.Application.ClockandUserContext.ICurrentUser, CurrentUserService>();
        services.AddScoped<SharedKernel.Application.Messaging.Outbox.IOutBoxMessageRepository, OutBoxRepository>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
            if (jwtSettings is not null)
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            }
        });

        if (!services.Any(x => x.ServiceType == typeof(IRabbitMqConnectionFactory)))
        {
            services.AddRabbitMqInfrastructure(configuration);
        }

        return services;
    }
}
