using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Application.Messaging.Outbox;
using SharedKernel.Infrastructure.Messaging.Outbox;

namespace SharedKernel.Infrastructure.Messaging.RabbitMQ;

public static class RabbitMqServiceCollectionExtensions
{
    /// <summary>
    /// Adds RabbitMQ infrastructure services
    /// </summary>
    public static IServiceCollection AddRabbitMqInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(
            configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
        services.AddSingleton<IRabbitMqTopologyInitializer, RabbitMqTopologyInitializer>();
        services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>();

        return services;
    }

    /// <summary>
    /// Adds RabbitMQ consumer for a specific module
    /// </summary>
    public static IServiceCollection AddRabbitMqConsumer(
        this IServiceCollection services,
        ModuleMessagingConfiguration configuration)
    {
        services.AddSingleton<IHostedService>(sp =>
            new RabbitMqConsumer(
                sp.GetRequiredService<IRabbitMqConnectionFactory>(),
                sp.GetRequiredService<IIntegrationEventDispatcher>(),
                configuration,
                sp.GetRequiredService<IOptions<RabbitMqOptions>>(),
                sp.GetRequiredService<ILogger<RabbitMqConsumer>>()));

        return services;
    }

    /// <summary>
    /// Adds outbox pattern support with Hangfire
    /// </summary>
    public static IServiceCollection AddOutboxPattern(
        this IServiceCollection services,
        string exchangeName,
        TimeSpan? processingInterval = null)
    {
        services.AddScoped<IOutBoxProcessor>(sp =>
        {
            var repository = sp.GetRequiredService<IOutBoxMessageRepository>();
            var publisher = sp.GetRequiredService<IRabbitMqPublisher>();
            var unitOfWork = sp.GetRequiredService<SharedKernel.Application.UNIT_OF_WORK.IUnitOfWork>();
            var logger = sp.GetRequiredService<ILogger<OutboxProcessor>>();

            return new OutboxProcessor(repository, publisher, unitOfWork, exchangeName, logger);
        });

        services.AddScoped<IOutboxJob, OutboxJob>();

        var interval = processingInterval ?? TimeSpan.FromSeconds(30);

        //RecurringJob.AddOrUpdate<IOutboxJob>(
        //    recurringJobId: $"outbox-processor-{exchangeName}",
        //    methodCall: job => job.ExecuteAsync(),
        //    cronExpression: $"*/{interval.TotalSeconds} * * * * *");

        return services;
    }

    /// <summary>
    /// Initializes RabbitMQ topology on application startup
    /// </summary>
    public static async Task InitializeRabbitMqTopologyAsync(
        this IHost host,
        ModuleMessagingConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        using var scope = host.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<IRabbitMqTopologyInitializer>();
        await initializer.InitializeAsync(configuration, cancellationToken);
    }
}
