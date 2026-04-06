using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure.Messaging.RabbitMQ;
using YouNaSchool.Notifications.Domain.Interfaces.Repositories;
using YouNaSchool.Notifications.Domain.Interfaces.Services;
using YouNaSchool.Notifications.Infrastructure.ExternalService;
using YouNaSchool.Notifications.Infrastructure.Persistance;
using YouNaSchool.Notifications.Infrastructure.Persistance.Repositories;
using YouNaSchool.Notifications.Infrastructure.Repositories;
using YouNaSchool.Notifications.Infrastructure.Settings;

namespace YouNaSchool.Notifications.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotificationInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region Connection To SQL SERVER
            var connectionString = configuration.GetConnectionString("Db");
            services.AddDbContext<NotificationDbContext>(options =>
            {
                options.UseSqlServer(
                    connectionString
                    );
            });
            #endregion

            #region Repos
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();
            #endregion

            #region EMAIL
            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.AddScoped<IEmailService, EmailService>();
            #endregion

            #region RabbitMQ
            services.AddRabbitMqInfrastructure(configuration);
            #endregion

            services.AddScoped<SharedKernel.Application.Messaging.Outbox.IOutBoxMessageRepository, OutBoxMessageRepository>();
            services.AddOutboxPattern(
                exchangeName: "notifications.events",
                processingInterval: TimeSpan.FromSeconds(30));

            return services;
        }
    }
}
