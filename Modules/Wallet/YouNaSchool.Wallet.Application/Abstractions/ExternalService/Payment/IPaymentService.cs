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
        /// <summary>
        /// Parses incoming webhook payload from any payment provider.
        /// </summary>
        /// <param name="provider">The payment provider (Stripe, PayPal, etc.)</param>
        /// <param name="payload">Raw request body</param>
        /// <param name="headers">Request headers</param>
        /// <returns>Result containing validation status and any error message</returns>
        PaymentWebhookResult ParseWebhook(PaymentProviders provider,string payload,IReadOnlyDictionary<string, string> headers);
    }
}
