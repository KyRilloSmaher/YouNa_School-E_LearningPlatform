using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Wallet.Application.Features.Commands.RechargeWallet;

namespace YouNaSchool.Wallet.Application.Validators
{
    public class RechargeWalletCommandValidator : AbstractValidator<RechargeWalletCommand>
    {
        public RechargeWalletCommandValidator()
        {
            RuleFor(x => x.WalletId)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .GreaterThan(50)
                .WithMessage("Recharge amount must be greater than 50EGP.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3);

            RuleFor(x => x.Provider)
                .IsInEnum();

            RuleFor(x => x.CallbackUrl)
                .NotEmpty()
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("CallbackUrl must be a valid absolute URL.");
        }
    }
}
