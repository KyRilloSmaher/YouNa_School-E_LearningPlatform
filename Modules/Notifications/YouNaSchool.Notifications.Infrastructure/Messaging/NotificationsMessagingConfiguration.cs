using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.IntegrationEvent;
using SharedKernel.Application.Messaging.Outbox;
using SharedKernel.Infrastructure.Messaging.Outbox;
using SharedKernel.Infrastructure.Messaging.RabbitMQ;
namespace YouNaSchool.Notifications.Infrastructure.Messaging
{
    public static class NotificationsMessagingConfiguration
    {
        public const string Exchange = "notifications.events";
        public const string RoutingKey = "notifications";
        public const string QueueName = "notifications.queue";
        public static IServiceCollection AddNotificationsMessaging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddRabbitMqInfrastructure(configuration);
            services.AddScoped<IIntegrationEventPublisher>(sp =>
                new OutboxIntegrationEventPublisher(
                    repository: sp.GetRequiredService<IOutBoxMessageRepository>(),
                    exchangeName: Exchange,
                    routingKeyPrefix: RoutingKey));
            services.AddOutboxPattern(Exchange, processingInterval: TimeSpan.FromSeconds(10));
            services.AddRabbitMqConsumer(BuildTopology());
            return services;
        }
        public static ModuleMessagingConfiguration BuildTopology() =>
            new()
            {
                ModuleName = "notifications",
                ExchangeName = Exchange,
                ExchangeType = "topic",
                QueueName = QueueName,
                Durable = true,
                AutoDelete = false,
                EnableDeadLetterQueue = true,
                RoutingKeyBindings =
                [
                    new("auth.events", "auth.SendConfirmationEmailIntegrationEvent"),
                    new("users.events", "users.UserCreatedIntegrationEvent"),
                    new("notifications.exchange", "notifications.lecture.paid"),
                    new("notifications.exchange", "notifications.wallet.recharged"),
                ]
            };
    }
}
