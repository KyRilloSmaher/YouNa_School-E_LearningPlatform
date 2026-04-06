using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Domain.Enums;

namespace YouNaSchool.Wallet.Application.Abstractions.ExternalService.Payment
{
    public interface IPaymentGatewayFactory
    {
        IPaymentService Resolve(PaymentProviders provider);
    }
}
