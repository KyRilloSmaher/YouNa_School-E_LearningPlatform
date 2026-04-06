using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Notifications.Domain.Models;

namespace YouNaSchool.Notifications.Infrastructure.Persistance.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Table name and schema
            builder.ToTable("Notifications", schema: "notifications");

            // Primary key
            builder.HasKey(n => n.Id);

            // Properties
            builder.Property(n => n.Id)
                   .ValueGeneratedNever(); // we generate Guid in domain

            builder.Property(n => n.UserId)
                   .IsRequired();

            builder.Property(n => n.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(n => n.Channel)
                   .HasConversion<int>() // store enum as int
                   .IsRequired();

            builder.Property(n => n.IsRead)
                   .IsRequired();

            // Optional: Indexes for queries
            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n => n.IsRead);

            // Ignore domain events property (if you have one in AggregateRoot)
            builder.Ignore("_domainEvents"); // assuming AggregateRoot has protected _domainEvents
        }
    }
}
