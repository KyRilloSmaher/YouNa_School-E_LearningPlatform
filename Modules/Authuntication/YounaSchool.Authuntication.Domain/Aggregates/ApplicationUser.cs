using Shared.Application.RESULT_PATTERN;
using SharedKernel.Domain.CoreAbstractions;
using SharedKernel.Domain.VALUE_OBJECTS;
using YounaSchool.Authuntication.Domain.Enums;
using YounaSchool.Authuntication.Domain.Events;

namespace YouNaSchool.Users.Domain.Entities
{
    public sealed class User : AggregateRoot
    {
        public Guid Id { get; private set; }
        public Email Email { get; private set; } = null!;
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public UserRole Role { get; private set; }

        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime? LastLogin { get; set; }

        private User() { }

        private User(Guid id, Email email, string firstName, string lastName, UserRole role)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = false;
            IsDeleted = false;

            RaiseDomainEvent(new UserRegisteredEvent(Id, email, Role));
        }

        public static Shared.Application.RESULT_PATTERN.Result<User> Register(Email email, string firstName, string lastName, UserRole role)
        {

             return  Result<User>.Failure("Email required" , System.Net.HttpStatusCode.BadRequest);

            var user = new User(
                Guid.NewGuid(),
                email,
                firstName.Trim(),
                lastName.Trim(),
                role);

            return Result<User>.Success(user);
        }

        public Result<bool> Activate()
        {
            if (IsActive) return Result<bool>.Failure("Already active" ,System.Net.HttpStatusCode.BadRequest);

            IsActive = true;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new UserActivatedEvent(Id,Email.Value));

            return Result<bool>.Success(true);
        }


        public Result<bool> Deactivate()
        {
            if (IsDeleted)
                return Result<bool>.Failure("User already deleted", System.Net.HttpStatusCode.BadRequest);

            IsActive = false;
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new UserDeactivatedEvent(Id, Email));

            return Result<bool>.Success(true);
        }
    }
}