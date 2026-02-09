using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Domain.Enums;
using static System.Collections.Specialized.BitVector32;

namespace YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment
{
    public interface IPaymentService
    {
        PaymentProviders Provider { get; }
        Task<PaymentSessionResult> CreateSessionAsync(CreatePaymentSessionCommand command,CancellationToken cancellationToken = default);

        Task CancelSessionAsync(string providerReferenceId,CancellationToken cancellationToken = default);

        PaymentWebhookResult ParseWebhook(PaymentProviders provider,string payload,IReadOnlyDictionary<string, string> headers);
    }
}
