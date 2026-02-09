namespace YouNaSchool.Wallet.Infrastructure.Messaging.Outbox
{
    public interface IOutboxJob
    {
        Task ExecuteAsync();
    }
}