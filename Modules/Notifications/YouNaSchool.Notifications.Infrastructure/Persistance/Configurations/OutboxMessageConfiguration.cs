using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouNaSchool.Notifications.Infrastructure.Persistance.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages" , schema: "notifications");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Type)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.Property(o => o.Payload)
                   .IsRequired();

            builder.Property(o => o.Error)
                   .HasColumnType("nvarchar(max)");
        }
    }
}
