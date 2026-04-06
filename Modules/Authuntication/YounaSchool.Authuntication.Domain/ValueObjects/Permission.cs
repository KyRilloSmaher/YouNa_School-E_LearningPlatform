using SharedKernel.Domain.CoreAbstractions;
using Shared.Application.RESULT_PATTERN;
using System.Net;

namespace YounaSchool.Authentication.Domain.ValueObjects;

/// <summary>
/// Represents a permission in the form of Resource/Action (e.g., \"lectures.read\").
/// </summary>
public sealed class Permission : ValueObject
{
    public string Resource { get; }
    public string Action { get; }

    public Permission() { }
    private Permission(string resource, string action)
    {
        Resource = resource;
        Action = action;
    }

    public string ToKey() => $"{Resource}.{Action}";

    public static Result<Permission> Create(string resource, string action)
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            return Result<Permission>.Failure("Permission resource is required.", HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(action))
        {
            return Result<Permission>.Failure("Permission action is required.", HttpStatusCode.BadRequest);
        }

        return Result<Permission>.Success(new Permission(
            resource.Trim().ToLowerInvariant(),
            action.Trim().ToLowerInvariant()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Resource;
        yield return Action;
    }
}

