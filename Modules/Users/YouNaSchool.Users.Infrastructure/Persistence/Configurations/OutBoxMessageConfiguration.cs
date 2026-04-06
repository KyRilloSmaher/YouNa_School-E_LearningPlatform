using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Application.OUTBOX_PATTERN;

namespace YouNaSchool.Users.Infrastructure.Persistence.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

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
