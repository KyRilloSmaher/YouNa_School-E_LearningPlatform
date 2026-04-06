using Auth.Application.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.IntegrationEvent;
using Shared.Application.IntegrationEvents;
using SharedKernel.Application.Messaging.Outbox;
using SharedKernel.Infrastructure.Messaging.Outbox;
using SharedKernel.Infrastructure.Messaging.RabbitMQ;

namespace Auth.Infrastructure.Messaging;

public static class AuthMessagingConfiguration
{
    // ─── Exchange / Queue topology ───────────────────────────────────────────
    public const string Exchange = "auth.events";
    public const string RoutingKey = "auth";
    public const string QueueName = "auth.queue";

    /// <summary>
    /// Call from Auth module's DI setup.
    /// </summary>
    public static IServiceCollection AddAuthMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Generic RabbitMQ infrastructure (idempotent)
        services.AddRabbitMqInfrastructure(configuration);

        // 2. Outbox-backed publisher scoped to the Auth exchange
        services.AddScoped<IIntegrationEventPublisher>(sp =>
            new OutboxIntegrationEventPublisher(
                repository: sp.GetRequiredService<IOutBoxMessageRepository>(),
                exchangeName: Exchange,
                routingKeyPrefix: RoutingKey));

        // 3. Outbox processor + Hangfire job
        services.AddOutboxPattern(Exchange, processingInterval: TimeSpan.FromSeconds(10));

        // 4. Consumer topology + hosted service
        var topology = BuildTopology();
        services.AddRabbitMqConsumer(topology);

        // 5. Auth module consumes Users events to react (e.g. pre-create auth record on UserCreated)
        
        return services;
    }

    public static ModuleMessagingConfiguration BuildTopology() =>
        new()
        {
            ModuleName = "auth",
            ExchangeName = Exchange,
            ExchangeType = "topic",
            QueueName = QueueName,
            Durable = true,
            AutoDelete = false,
            EnableDeadLetterQueue = true,
            RoutingKeyBindings = []
        };
}
