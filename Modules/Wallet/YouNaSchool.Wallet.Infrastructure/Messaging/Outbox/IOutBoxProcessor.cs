namespace YouNaSchool.Wallet.Infrastructure.Messaging.Outbox
{
    public interface IOutBoxProcessor
    {
        Task ProcessAsync(CancellationToken cancellationToken = default);
    }
}