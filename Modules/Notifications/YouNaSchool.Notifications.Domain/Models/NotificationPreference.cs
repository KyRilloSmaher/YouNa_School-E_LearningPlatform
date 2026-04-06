using SharedKernel.Domain.CoreAbstractions;
using YouNaSchool.Notifications.Domain.Enums;

namespace YouNaSchool.Notifications.Domain.Models
{
    public sealed class NotificationPreference : AggregateRoot
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public NotificationChannel Channel { get; private set; }


        public bool IsEnabled { get; private set; }

        private NotificationPreference() { }

        public NotificationPreference(
            Guid userId,
            NotificationChannel channel,
            bool isEnabled)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Channel = channel;
            IsEnabled = isEnabled;
        }

        public void Update(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
    }
}