using Hangfire;

namespace YouNaSchool.Wallet.Infrastructure.Messaging.Outbox
{
    internal sealed class OutboxJob : IOutboxJob
    {
        private readonly IOutBoxProcessor _processor;

        public OutboxJob(IOutBoxProcessor processor)
        {
            _processor = processor;
        }

        [AutomaticRetry(Attempts = 5)]
        public async Task ExecuteAsync()
        {
            await _processor.ProcessAsync();
        }
    }
}