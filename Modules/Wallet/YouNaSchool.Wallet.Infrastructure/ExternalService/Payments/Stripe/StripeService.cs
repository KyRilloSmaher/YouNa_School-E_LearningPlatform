using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment;
using YouNaSchool.Wallet.Domain.Enums;
using YouNaSchool.Wallet.Infrastructure.Settings;

namespace YouNaSchool.Wallet.Infrastructure.ExternalService.Payments.Stripe
{
    public class StripeService : IPaymentService
    {
        public PaymentProviders Provider => PaymentProviders.Stripe;

        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<StripeService> _logger;


        public StripeService(IOptions<StripeSettings> stripeSettings, ILogger<StripeService> logger)
        {
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            _logger = logger;
        }

        public async Task<PaymentSessionResult> CreateSessionAsync(CreatePaymentSessionCommand command,CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();

            var intent = await service.CreateAsync(
                new PaymentIntentCreateOptions
                {
                    Amount = Convert.ToInt64(command.Amount * 100), // cents
                    Currency = command.Currency.ToLower(),
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        ["walletId"] = command.WalletId.ToString(),
                        ["rechargeId"] = command.RechargeId.ToString()
                    }
                },
                cancellationToken: cancellationToken
            );

            return new PaymentSessionResult
            {
                Provider = PaymentProviders.Stripe,
                ProviderReferenceId = intent.Id,
                ClientPaymentToken = intent.ClientSecret
            };
        }

        public async Task CancelSessionAsync(string providerReferenceId,CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();

            await service.CancelAsync(
                providerReferenceId,
                cancellationToken: cancellationToken
            );
        }

        public PaymentWebhookResult ParseWebhook(PaymentProviders provider,string payload,IReadOnlyDictionary<string, string> headers)
        {
            if (provider != PaymentProviders.Stripe)
                throw new InvalidOperationException("Invalid provider for Stripe gateway");

            if (!headers.TryGetValue("Stripe-Signature", out var signature))
                return new PaymentWebhookResult { IsValid = false };

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    payload,
                    signature,
                    _stripeSettings.WebhookSecret
                );
            }
            catch
            {
                return new PaymentWebhookResult { IsValid = false };
            }

            if (stripeEvent.Data.Object is not PaymentIntent intent)
                return new PaymentWebhookResult { IsValid = false };

            return new PaymentWebhookResult
            {
                IsValid = true,
                Provider = PaymentProviders.Stripe,
                ProviderReferenceId = intent.Id,
                Status = MapStatus(intent.Status),
                Amount = intent.AmountReceived / 100m
            };
        }

        private static RechargeStatus MapStatus(string stripeStatus) =>
            stripeStatus switch
            {
                "succeeded" => RechargeStatus.Completed,
                "canceled" => RechargeStatus.Failed,
                "requires_payment_method" => RechargeStatus.Failed,
                _ => RechargeStatus.Pending
            };
    }
}
