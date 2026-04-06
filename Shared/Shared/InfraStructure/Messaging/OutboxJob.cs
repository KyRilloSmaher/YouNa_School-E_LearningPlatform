using Hangfire;
using Microsoft.Extensions.Logging;
using SharedKernel.Application.Messaging.Outbox;

namespace SharedKernel.Infrastructure.Messaging.Outbox;


internal sealed class OutboxJob : IOutboxJob
{
    private readonly IOutBoxProcessor _processor;
    private readonly ILogger<OutboxJob> _logger;

    public OutboxJob(
        IOutBoxProcessor processor,
        ILogger<OutboxJob> logger)
    {
        _processor = processor;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 30, 60 })]
    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task ExecuteAsync()
    {
        _logger.LogDebug("Starting outbox job execution");

        try
        {
            await _processor.ProcessAsync();

            _logger.LogDebug("Outbox job execution completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing outbox job");
            throw;
        }
    }
}