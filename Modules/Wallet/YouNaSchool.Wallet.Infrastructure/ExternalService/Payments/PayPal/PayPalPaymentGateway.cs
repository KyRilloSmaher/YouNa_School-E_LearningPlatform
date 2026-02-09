
using System.Text;
using System.Text.Json;
using YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment;
using YouNaSchool.Wallet.Domain.Enums;
using YouNaSchool.Wallet.Infrastructure.Settings;

namespace YouNaSchool.Wallet.Infrastructure.ExternalService.Payments.PayPal
{
    public sealed class PayPalPaymentGateway : IPaymentService
    {
        public PaymentProviders Provider => PaymentProviders.PayPal;
        private readonly HttpClient _http;
        private readonly PayPalAuthService _auth;
        private readonly PayPalSettings _settings;


        public PayPalPaymentGateway(HttpClient http,PayPalAuthService auth,PayPalSettings settings)
        {
            _http = http;
            _auth = auth;
            _settings = settings;
        }

        public async Task<PaymentSessionResult> CreateSessionAsync(
            CreatePaymentSessionCommand command,
            CancellationToken cancellationToken = default)
        {
            var token = await _auth.GetAccessTokenAsync(cancellationToken);

            var request = new HttpRequestMessage(HttpMethod.Post, "/v2/checkout/orders");
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            request.Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    intent = "CAPTURE",
                    purchase_units = new[]
                    {
                        new
                        {
                            amount = new
                            {
                                currency_code = command.Currency,
                                value = command.Amount.ToString("F2")
                            },
                            reference_id = command.RechargeId.ToString()
                        }
                    },
                    application_context = new
                    {
                        return_url = command.CallbackUrl,
                        cancel_url = command.CallbackUrl
                    }
                }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _http.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            var orderId = doc.RootElement.GetProperty("id").GetString()!;
            var approveUrl = doc.RootElement
                .GetProperty("links")
                .EnumerateArray()
                .First(x => x.GetProperty("rel").GetString() == "approve")
                .GetProperty("href")
                .GetString();

            return new PaymentSessionResult
            {
                Provider = PaymentProviders.PayPal,
                ProviderReferenceId = orderId,
                ClientPaymentToken = approveUrl
            };
        }

        public async Task CancelSessionAsync(
            string providerReferenceId,
            CancellationToken cancellationToken = default)
        {
            // PayPal orders cannot be "canceled" like Stripe intents
            // They expire automatically — no-op by design
            await Task.CompletedTask;
        }

        public PaymentWebhookResult ParseWebhook(
            PaymentProviders provider,
            string payload,
            IReadOnlyDictionary<string, string> headers)
        {
            if (provider != PaymentProviders.PayPal)
                throw new InvalidOperationException("Invalid provider for PayPal gateway");

            if (!headers.TryGetValue("PayPal-Transmission-Id", out _))
                return new PaymentWebhookResult { IsValid = false };

            using var doc = JsonDocument.Parse(payload);

            var eventType = doc.RootElement.GetProperty("event_type").GetString();
            var resource = doc.RootElement.GetProperty("resource");

            var orderId = resource.GetProperty("id").GetString();

            return new PaymentWebhookResult
            {
                IsValid = true,
                Provider = PaymentProviders.PayPal,
                ProviderReferenceId = orderId!,
                Status = MapStatus(eventType!)
            };
        }

        private static RechargeStatus MapStatus(string eventType) =>
            eventType switch
            {
                "CHECKOUT.ORDER.APPROVED" => RechargeStatus.Completed,
                "PAYMENT.CAPTURE.COMPLETED" => RechargeStatus.Completed,
                "CHECKOUT.ORDER.CANCELLED" => RechargeStatus.Failed,
                _ => RechargeStatus.Pending
            };
    }
}
