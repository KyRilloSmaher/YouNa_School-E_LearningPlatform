using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Application.Abstractions.Messaging
{
    public interface IIntegrationEventHandler
    {
        Task HandleAsync(
        string messageType,
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken);
    }
}
