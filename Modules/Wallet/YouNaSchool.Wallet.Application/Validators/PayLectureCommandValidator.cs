
using FluentValidation;
using YouNaSchool.Wallet.Application.Features.Commands.PayLecture;

namespace YouNaSchool.Wallet.Application.Validators
{
    public sealed class PayLectureCommandValidator
        : AbstractValidator<PayLectureCommand>
    {
        public PayLectureCommandValidator()
        {
            RuleFor(x => x.WalletId)
                .NotEmpty();

            RuleFor(x => x.StudentId)
                .NotEmpty();

            RuleFor(x => x.LectureId)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .NotNull();

            RuleFor(x => x.Amount.Amount)
                .GreaterThan(50)
                .WithMessage("Payment amount must be greater than ZERO");

            RuleFor(x => x.Amount.Currency)
                .NotEmpty()
                .Length(3);
        }
    }
}