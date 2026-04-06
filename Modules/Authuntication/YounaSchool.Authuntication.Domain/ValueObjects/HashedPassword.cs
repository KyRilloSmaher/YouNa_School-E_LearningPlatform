
using SharedKernel.Domain.CoreAbstractions;
using Shared.Application.RESULT_PATTERN;
using System.Net;

namespace YounaSchool.Authentication.Domain.ValueObjects;

/// <summary>
/// Represents a one-way hashed password value.
/// </summary>
public sealed class HashedPassword : ValueObject
{
    /// <summary>
    /// Gets the hashed password value.
    /// </summary>
    public string Value { get; }

    public HashedPassword() { }
    private HashedPassword(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new <see cref="HashedPassword"/> instance after validating the hash.
    /// </summary>
    /// <param name="hash">The password hash produced by the password hashing service.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the created <see cref="HashedPassword"/> when valid,
    /// otherwise a failure result with the validation error.
    /// </returns>
    public static Result<HashedPassword> Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            return Result<HashedPassword>.Failure("Password hash cannot be empty.", HttpStatusCode.BadRequest);
        }

        // Additional invariants (length, format) can be added here if needed.
        var value = new HashedPassword(hash);
        return Result<HashedPassword>.Success(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}