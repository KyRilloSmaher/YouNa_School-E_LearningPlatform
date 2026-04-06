using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment;
using YouNaSchool.Wallet.Application.Features.Commands.RechargeCompleted;
using YouNaSchool.Wallet.Application.Features.Commands.RechargeFailed;
using YouNaSchool.Wallet.Domain.Enums;

namespace YouNaSchool.Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentWebHooks : ControllerBase
    {
        private readonly ILogger<PaymentWebHooks> _logger;
        private readonly IConfiguration _configuration;
        private readonly IPaymentGatewayFactory _paymentGatewayFactory;
        private readonly IMediator _mediator;

        public PaymentWebHooks(
            ILogger<PaymentWebHooks> logger,
            IConfiguration configuration,
            IPaymentGatewayFactory paymentGatewayFactory,
            IMediator mediator)
        {
            _logger = logger;
            _configuration = configuration;
            _paymentGatewayFactory = paymentGatewayFactory;
            _mediator = mediator;
        }

        [HttpPost("{provider}-webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> WebhookAsync(string provider)
        {
            // Try parse provider dynamically
            if (!Enum.TryParse<PaymentProviders>(provider, true, out var paymentProvider))
            {
                _logger.LogWarning("Unknown payment provider webhook: {Provider}", provider);
                return BadRequest("Unknown provider");
            }

            var paymentService = _paymentGatewayFactory.Resolve(paymentProvider);

            _logger.LogInformation("Processing {Provider} webhook", paymentProvider);

            var payload = await new StreamReader(Request.Body).ReadToEndAsync();

            // Convert headers to dictionary
            var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            try
            {
                var result = paymentService.ParseWebhook(paymentProvider, payload, headers);

                if (!result.IsValid)
                {
                    _logger.LogWarning("Webhook validation failed: {Error}", result.Status);
                    return BadRequest(result.Status);
                }
                if (result.Status == RechargeStatus.Failed)
                {
                    var response = await _mediator.Send(new RechargeFailedCommand(result.RechargeId, result.ProviderReferenceId));
                    return response.IsSuccess ? Ok() : BadRequest(response.Error);
                }
                else if (result.Status == RechargeStatus.Completed)
                {
                    var response = await _mediator.Send(new RechargeCompletedCommand(result.RechargeId, result.ProviderReferenceId));
                    return response.IsSuccess ? Ok() : BadRequest(response.Error);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled webhook processing error for {Provider}", paymentProvider);
                return Ok();
            }
        }
    }
}