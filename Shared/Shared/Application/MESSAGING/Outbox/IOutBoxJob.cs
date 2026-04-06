namespace SharedKernel.Application.Messaging.Outbox
{
    /// <summary>
    /// Hangfire job wrapper for outbox processing
    /// </summary>
    public interface IOutboxJob
    {
        Task ExecuteAsync();
    }
}