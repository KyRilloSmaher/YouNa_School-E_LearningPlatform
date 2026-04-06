using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouNaSchool.Notifications.Domain.Models;
using YouNaSchool.Notifications.Domain.Enums;

namespace YouNaSchool.Notifications.Infrastructure.Persistence.Configurations
{
    public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
    {
        public void Configure(EntityTypeBuilder<NotificationPreference> builder)
        {
            // Table name and schema
            builder.ToTable("NotificationPreferences", schema: "notifications");

            // Primary key
            builder.HasKey(np => np.Id);

            builder.Property(np => np.Id)
                   .ValueGeneratedNever(); // Guid generated in domain

            // Properties
            builder.Property(np => np.UserId)
                   .IsRequired();

            builder.Property(np => np.Channel)
                   .HasConversion<int>() // store enum as int
                   .IsRequired();

            builder.Property(np => np.IsEnabled)
                   .IsRequired();

            // Index for fast lookup by user
            builder.HasIndex(np => np.UserId);

            // Ignore domain events from AggregateRoot
            builder.Ignore("_domainEvents");
        }
    }
}