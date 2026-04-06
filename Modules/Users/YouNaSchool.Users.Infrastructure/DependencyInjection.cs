using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application.UNIT_OF_WORK;
using SharedKernel.Infrastructure.Messaging.RabbitMQ;
using UsersRabbitMqPublisherContract = YouNaSchhol.Users.Application.Abstractions.Messaging.RabbitMq.IRabbitMqPublisher;
using YouNaSchool.Users.Application.Abstractions.ExternalService;
using YouNaSchool.Users.Application.Abstractions.Persistence;
using YouNaSchool.Users.Infrastructure.ExternalServices;
using YouNaSchool.Users.Infrastructure.Messaging;
using YouNaSchool.Users.Infrastructure.Persistence;
using YouNaSchool.Users.Infrastructure.Persistence.Repositories;
using YouNaSchool.Users.Infrastructure.Settings;


namespace YouNaSchool.Users.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region Connection To SQL SERVER
            var connectionString = configuration.GetConnectionString("Db")
                ?? configuration.GetConnectionString("Db")
                ?? configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'UsersDb' was not found.");
            }

            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseSqlServer(
                    connectionString
                    );
            });
            #endregion

            #region EMAIL
            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.AddScoped<IEmailService, EmailService>();
            #endregion

            #region Persistence
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IAssistantRepository, AssistantRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            #endregion

            // Add RabbitMQ infrastructure
            services.AddRabbitMqInfrastructure(configuration);
            services.AddSingleton<UsersRabbitMqPublisherContract, UsersRabbitMqPublisher>();

            // Configure Users module messaging
            var usersMessagingConfig = new ModuleMessagingConfiguration
            {
                ModuleName = "users",
                ExchangeName = "users.events",
                ExchangeType = "topic",
                QueueName = "users.internal.queue",
                RoutingKeyBindings = new List<RoutingKeyBinding>
            {
                // Users can listen to its own events if needed for internal processing
                new RoutingKeyBinding("users.events", "users.*")
            },
                Durable = true,
                AutoDelete = false,
                EnableDeadLetterQueue = true
            };

            // Add consumer (optional - only if Users module needs to consume its own events)
            // services.AddRabbitMqConsumer(usersMessagingConfig);

            // Add outbox pattern support
            services.AddScoped<SharedKernel.Application.Messaging.Outbox.IOutBoxMessageRepository, OutBoxMessageRepository>();
            services.AddOutboxPattern(
                exchangeName: "users.events",
                processingInterval: TimeSpan.FromSeconds(30));


            return services;
        }
    }
}
