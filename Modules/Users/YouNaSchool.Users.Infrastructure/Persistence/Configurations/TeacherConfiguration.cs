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
    public sealed class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.ToTable("Teachers");

            builder.HasKey(x => x.Id);
            builder.Property(t => t.IsVerified)
                          .IsRequired();

            builder.HasIndex(x => x.UserId)
                    .IsUnique();
        }
    }
}
