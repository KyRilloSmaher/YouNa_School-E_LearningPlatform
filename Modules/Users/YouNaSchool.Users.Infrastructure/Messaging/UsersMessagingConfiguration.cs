using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.IntegrationEvent;
using Shared.Application.IntegrationEvents;
using SharedKernel.Application.Messaging.Outbox;
using SharedKernel.Infrastructure.Messaging.Outbox;
using SharedKernel.Infrastructure.Messaging.RabbitMQ;
using YounaSchool.Authuntication.Application.IntegrationEvents;
using YouNaSchool.Users.Infrastructure.IntegrationEvents.IntegrationEventsHandlers;
namespace Users.Infrastructure.Messaging;

public static class UsersMessagingConfiguration
{
    // ─── Exchange / Queue topology ───────────────────────────────────────────
    public const string Exchange = "users.events";
    public const string RoutingKey = "users";           // prefix → "users.<EventName>"
    public const string QueueName = "users.queue";     // Users module consumes its OWN events too (e.g. saga)

    /// <summary>
    /// Call from Users module's DI setup.
    /// Registers the publisher (outbox-backed) and consumer handlers.
    /// </summary>
    public static IServiceCollection AddUsersMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Generic RabbitMQ infrastructure (idempotent — safe to call multiple times)
        services.AddRabbitMqInfrastructure(configuration);

        // 2. Outbox-backed publisher scoped to the Users exchange
        services.AddScoped<IIntegrationEventPublisher>(sp =>
            new OutboxIntegrationEventPublisher(
                repository: sp.GetRequiredService<IOutBoxMessageRepository>(),
                exchangeName: Exchange,
                routingKeyPrefix: RoutingKey));

        // 3. Outbox processor + Hangfire job scoped to this exchange
        services.AddOutboxPattern(Exchange, processingInterval: TimeSpan.FromSeconds(10));

        // 4. Consumer topology + hosted service
        var topology = BuildTopology();
        services.AddRabbitMqConsumer(topology);

        // 5. Register event handlers for events THIS module consumes
        services.AddScoped<IIntegrationEventHandler<AuthUserConfirmedIntegrationEvent>, AuthUserConfirmedIntegrationEventHandler>();
        return services;
    }

    public static ModuleMessagingConfiguration BuildTopology() =>
        new()
        {
            ModuleName = "users",
            ExchangeName = Exchange,
            ExchangeType = "topic",
            QueueName = QueueName,
            Durable = true,
            AutoDelete = false,
            EnableDeadLetterQueue = true,
            RoutingKeyBindings =
            [
                new("auth.events", "auth.AuthUserConfirmedIntegrationEvent"),
            ]
        };
}
