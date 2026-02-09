using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouNaSchool.Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Persistence.Configurations
{
    internal sealed class WalletRechargeConfiguration
        : IEntityTypeConfiguration<WalletRecharge>
    {
        public void Configure(EntityTypeBuilder<WalletRecharge> builder)
        {
            builder.ToTable("WalletRecharges");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.WalletId)
                   .IsRequired();

            builder.Property(r => r.Status)
                   .IsRequired();

            builder.Property(r => r.CreatedAt)
                   .IsRequired();

            builder.Property(r => r.CompletedAt);

            builder.OwnsOne(r => r.Amount, money =>
            {
                money.Property(m => m.Amount)
                     .HasPrecision(18, 2)
                     .IsRequired();

                money.Property(m => m.Currency)
                     .HasMaxLength(3)
                     .IsRequired();
            });

            builder.HasIndex(r => r.WalletId);
        }
    }
}