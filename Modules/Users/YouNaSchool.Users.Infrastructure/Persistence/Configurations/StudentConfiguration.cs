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
    public sealed class UserConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();
            builder.Property(x => x.AssistantId)
                .IsRequired(false);
            builder.Property(x => x.Level)
                .IsRequired();
            builder.Property(x => x.IsSuspended)
                .IsRequired();
            builder.Property(x => x.SuspensionCount)
                .IsRequired();

            builder.HasIndex(x => x.UserId)
                .IsUnique();


        }
    }
}
