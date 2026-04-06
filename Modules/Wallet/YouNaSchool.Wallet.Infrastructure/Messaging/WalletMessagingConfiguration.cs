using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.IntegrationEvent;
using Shared.Application.IntegrationEvents;
using SharedKernel.Application.Messaging.Outbox;
using SharedKernel.Infrastructure.Messaging.Outbox;
using SharedKernel.Infrastructure.Messaging.RabbitMQ;
namespace Wallet.Infrastructure.Messaging;

public static class WalletMessagingConfiguration
{
    // ─── Exchange / Queue topology ───────────────────────────────────────────
    public const string Exchange = "wallet.events";
    public const string RoutingKey = "wallet";
    public const string QueueName = "wallet.queue";

    /// <summary>
    /// Call from Wallet module's DI setup.
    /// </summary>
    public static IServiceCollection AddWalletMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Generic RabbitMQ infrastructure (idempotent)
        services.AddRabbitMqInfrastructure(configuration);

        // 2. Outbox-backed publisher scoped to the Wallet exchange
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

        // 5. Wallet listens to Users events — create a wallet when a user is created
        //services.AddScoped<
        //    IIntegrationEventHandler<UserCreatedIntegrationEvent>,UserCreatedHandler>();

        return services;
    }

    public static ModuleMessagingConfiguration BuildTopology() =>
        new()
        {
            ModuleName = "wallet",
            ExchangeName = Exchange,
            ExchangeType = "topic",
            QueueName = QueueName,
            Durable = true,
            AutoDelete = false,
            EnableDeadLetterQueue = true,
            RoutingKeyBindings =
            [
                // Wallet listens to Users events to auto-create wallets
                new("users.events", "users.StudentCreatedIntegrationEvent"),
            ]
        };
}
