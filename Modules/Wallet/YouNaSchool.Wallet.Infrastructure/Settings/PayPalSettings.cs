using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Wallet.Infrastructure.Settings
{
    public sealed class PayPalSettings
    {
        public string ClientId { get; init; } = default!;
        public string ClientSecret { get; init; } = default!;
        public string BaseUrl { get; init; } = "https://api-m.sandbox.paypal.com";
        public string WebhookId { get; init; } = default!;
    }
}
