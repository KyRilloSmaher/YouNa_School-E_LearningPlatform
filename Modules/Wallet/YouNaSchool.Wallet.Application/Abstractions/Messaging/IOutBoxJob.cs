namespace YouNaSchool.Wallet.Application.Messaging
{
    public interface IOutboxJob
    {
        Task ExecuteAsync();
    }
}