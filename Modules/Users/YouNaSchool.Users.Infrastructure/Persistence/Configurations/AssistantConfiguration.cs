using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouNaSchool.Users.Domain.Entities;

namespace YouNaSchool.Users.Infrastructure.Persistence.Configurations
{
    public sealed class AssistantConfiguration : IEntityTypeConfiguration<Assistant>
    {
        public void Configure(EntityTypeBuilder<Assistant> builder)
        {
            builder.ToTable("Assistants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.TeacherId).IsRequired();
            builder.HasIndex(x => x.UserId)
                    .IsUnique();
        }
    }
}
