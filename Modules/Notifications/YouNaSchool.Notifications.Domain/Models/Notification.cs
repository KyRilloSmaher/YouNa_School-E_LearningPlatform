using SharedKernel.Domain.CoreAbstractions;
using YouNaSchool.Notifications.Domain.Domain_Events;
using YouNaSchool.Notifications.Domain.Domain_Events.YouNaSchool.Notifications.Domain.Events;
using YouNaSchool.Notifications.Domain.Enums;

namespace YouNaSchool.Notifications.Domain.Models
{
    public class Notification : AggregateRoot
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }

        public string Title { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;

        public NotificationChannel Channel { get; private set; }
        public bool IsRead { get; private set; }

        private Notification() { } // EF Core

        private Notification(
            Guid id,
            Guid userId,
            string title,
            string message,
            NotificationChannel channel)
        {
            Id = id;
            UserId = userId;
            Title = title;
            Message = message;
            Channel = channel;
            IsRead = false;
        }

        public static Notification Create(
            Guid userId,
            NotificationChannel channel,
            string title,
            string message, string? phone, string? Email)
        {
            var notification = new Notification(
                 Guid.NewGuid(),
                 userId,
                 title,
                 message,
                 channel);

            notification.RaiseDomainEvent(
                new NotificationCreatedEvent(

                    notification.Id,
                    userId,
                    title,
                    message ,channel,phone,Email));

            return notification;
        }

        public void MarkAsRead()
        {
            if (IsRead) return;

            IsRead = true;
            RaiseDomainEvent(new NotificationReadEvent(Id, UserId));
        }
    }
}