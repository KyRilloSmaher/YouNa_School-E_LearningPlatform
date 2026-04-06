using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Infrastructure.Settings;

namespace YouNaSchool.Wallet.Infrastructure.ExternalService.Payments.PayPal
{
    public sealed class PayPalAuthService
    {
        private readonly HttpClient _http;
        private readonly PayPalSettings _settings;

        public PayPalAuthService(IOptions<PayPalSettings> options, HttpClient http)
        {
            _settings = options.Value;
            _http = http;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken ct)
        {
            var auth = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}")
            );

            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
            request.Content = new StringContent("grant_type=client_credentials",
                Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await _http.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement.GetProperty("access_token").GetString()!;
        }
    }
}
