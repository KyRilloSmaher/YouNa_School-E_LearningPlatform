using SharedKernel.Domain.CoreAbstractions;
using Shared.Application.RESULT_PATTERN;
using System.Net;
using System.Text.RegularExpressions;

namespace YounaSchool.Authentication.Domain.ValueObjects;

/// <summary>
/// Email value object with basic validation and normalization.
/// </summary>
public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    public Email() { }
    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Email>.Failure("Email is required.", HttpStatusCode.BadRequest);
        }

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
        {
            return Result<Email>.Failure("Invalid email format.", HttpStatusCode.BadRequest);
        }

        return Result<Email>.Success(new Email(normalized));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

