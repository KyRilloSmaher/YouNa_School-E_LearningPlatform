using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouNaSchool.Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Persistence.Configurations
{
    internal sealed class WalletLedgerEntryConfiguration
        : IEntityTypeConfiguration<WalletLedgerEntry>
    {
        public void Configure(EntityTypeBuilder<WalletLedgerEntry> builder)
        {
            builder.ToTable("WalletLedgerEntries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.WalletId)
                   .IsRequired();

            builder.Property(x => x.TransactionType)
                   .IsRequired();

            builder.Property(x => x.Source)
                   .IsRequired();

            builder.Property(x => x.ReferenceId)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.OwnsOne(x => x.Amount, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("Amount")
                     .HasPrecision(18, 2)
                     .IsRequired();

                money.Property(m => m.Currency)
                     .HasColumnName("Currency")
                     .HasMaxLength(3)
                     .IsRequired();
            });

            builder.HasIndex(x => x.WalletId);
        }
    }
}