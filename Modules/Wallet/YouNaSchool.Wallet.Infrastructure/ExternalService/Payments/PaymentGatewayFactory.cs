using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment;
using YouNaSchool.Wallet.Domain.Enums;

namespace YouNaSchool.Wallet.Infrastructure.ExternalService.Payments
{
    public sealed class PaymentGatewayFactory : IPaymentGatewayFactory
    {
        private readonly IEnumerable<IPaymentService> _gateways;

        public PaymentGatewayFactory(IEnumerable<IPaymentService> gateways)
        {
            _gateways = gateways;
        }

        public IPaymentService Resolve(PaymentProviders provider)
        {
            var gateway = _gateways.FirstOrDefault(g =>g.Provider == provider);

            if (gateway is null)
                throw new NotSupportedException($"Payment provider '{provider}' is not supported.");

            return gateway;
        }
    }
}
