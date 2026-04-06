namespace YouNaSchool.Wallet.Application.Messaging
{
    public interface IOutBoxProcessor
    {
        Task ProcessAsync(CancellationToken cancellationToken = default);
    }
}