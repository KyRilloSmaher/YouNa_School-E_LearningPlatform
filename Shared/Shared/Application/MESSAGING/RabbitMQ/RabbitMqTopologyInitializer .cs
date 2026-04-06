using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Threading.Channels;

namespace SharedKernel.Infrastructure.Messaging.RabbitMQ;

/// <summary>
/// Service for initializing RabbitMQ topology (exchanges, queues, bindings)
/// </summary>
public interface IRabbitMqTopologyInitializer
{
    Task InitializeAsync(
        ModuleMessagingConfiguration configuration,
        CancellationToken cancellationToken = default);
}

internal sealed class RabbitMqTopologyInitializer : IRabbitMqTopologyInitializer
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqTopologyInitializer> _logger;

    public RabbitMqTopologyInitializer(
        IRabbitMqConnectionFactory connectionFactory,
        Microsoft.Extensions.Options.IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqTopologyInitializer> logger)
    {
        _connectionFactory = connectionFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InitializeAsync(
        ModuleMessagingConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Initializing RabbitMQ topology for module: {ModuleName}",
            configuration.ModuleName);

        var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        try
        {
            // 1. Declare the module's exchange
            await DeclareExchangeAsync(channel, configuration);

            // 2. Declare dead letter exchange if enabled
            if (configuration.EnableDeadLetterQueue)
            {
                await DeclareDeadLetterExchangeAsync(channel);
            }

            // 3. Declare the main queue
            await DeclareQueueAsync(channel, configuration);

            // 4. Declare dead letter queue if enabled
            if (configuration.EnableDeadLetterQueue)
            {
                await DeclareDeadLetterQueueAsync(channel, configuration);
            }

            // 5. Bind queue to exchanges with routing keys
            await BindQueueToExchangesAsync(channel, configuration);

            _logger.LogInformation(
                "Successfully initialized RabbitMQ topology for module: {ModuleName}",
                configuration.ModuleName);
        }
        finally
        {
            await channel.CloseAsync(cancellationToken);
        }
    }

    private async Task DeclareExchangeAsync(
        IChannel channel,
        ModuleMessagingConfiguration configuration)
    {
        _logger.LogDebug(
            "Declaring exchange: {ExchangeName} (Type: {ExchangeType})",
            configuration.ExchangeName,
            configuration.ExchangeType);

        await channel.ExchangeDeclareAsync(
            exchange: configuration.ExchangeName,
            type: configuration.ExchangeType,
            durable: configuration.Durable,
            autoDelete: configuration.AutoDelete,
            arguments: null);
    }

    private async Task DeclareDeadLetterExchangeAsync(IChannel channel)
    {
        _logger.LogDebug(
            "Declaring dead letter exchange: {ExchangeName}",
            _options.DeadLetterExchange);

        await channel.ExchangeDeclareAsync(
            exchange: _options.DeadLetterExchange,
            type: "topic",
            durable: true,
            autoDelete: false,
            arguments: null);
    }

    private async Task DeclareQueueAsync(
        IChannel channel,
        ModuleMessagingConfiguration configuration)
    {
        _logger.LogDebug("Declaring queue: {QueueName}", configuration.QueueName);

        var arguments = new Dictionary<string, object?>();

        if (configuration.EnableDeadLetterQueue)
        {
            arguments["x-dead-letter-exchange"] = _options.DeadLetterExchange;
            arguments["x-dead-letter-routing-key"] = $"{configuration.ModuleName}.dlq";
        }

        await channel.QueueDeclareAsync(
            queue: configuration.QueueName,
            durable: configuration.Durable,
            exclusive: false,
            autoDelete: configuration.AutoDelete,
            arguments: arguments);
    }

    private async Task DeclareDeadLetterQueueAsync(
        IChannel channel,
        ModuleMessagingConfiguration configuration)
    {
        _logger.LogDebug(
            "Declaring dead letter queue: {QueueName}",
            configuration.DeadLetterQueueName);

        await channel.QueueDeclareAsync(
            queue: configuration.DeadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        await channel.QueueBindAsync(
            queue: configuration.DeadLetterQueueName,
            exchange: _options.DeadLetterExchange,
            routingKey: $"{configuration.ModuleName}.dlq");
    }

    private async Task BindQueueToExchangesAsync(
    IChannel channel,
    ModuleMessagingConfiguration configuration)
    {
        foreach (var binding in configuration.RoutingKeyBindings)
        {
            _logger.LogDebug(
                "Binding queue {QueueName} to exchange {ExchangeName} with routing key {RoutingKey}",
                configuration.QueueName,
                binding.ExchangeName,
                binding.RoutingKey);

            // ✅ Ensure the external exchange exists before binding
            await channel.ExchangeDeclareAsync(
                exchange: binding.ExchangeName,
                type: ExchangeType.Topic,   // Assuming all cross-module exchanges are topic
                durable: true,
                autoDelete: false,
                arguments: null);

            // Bind the queue
            await channel.QueueBindAsync(
                queue: configuration.QueueName,
                exchange: binding.ExchangeName,
                routingKey: binding.RoutingKey);
        }
    }
}