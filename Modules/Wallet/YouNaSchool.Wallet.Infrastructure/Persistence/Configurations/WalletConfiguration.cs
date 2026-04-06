using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouNaSchool.Wallet.Domain.Entities;
using SharedKernel.Domain.VALUE_OBJECTS;

namespace YouNaSchool.Wallet.Infrastructure.Persistence.Configurations
{
    internal sealed class WalletConfiguration : IEntityTypeConfiguration<Wallets>
    {
        public void Configure(EntityTypeBuilder<Wallets> builder)
        {
            builder.ToTable("Wallets" ,schema: "Wallets");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.StudentId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(w => w.IsActive)
                   .IsRequired();

            builder.Property(w => w.CreatedAt)
                   .IsRequired();
            builder.HasIndex(w => w.StudentId)
               .IsUnique();

            builder.Ignore(w => w.DomainEvents);
            // ============================
            // Money Value Object
            // ============================
            builder.OwnsOne(w => w.Balance, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("BalanceAmount")
                     .HasPrecision(18, 2)
                     .IsRequired();

                money.Property(m => m.Currency)
                     .HasColumnName("BalanceCurrency")
                     .HasMaxLength(3)
                     .IsRequired();
            });

            // ============================
            // Ledger Entries (Aggregate Child)
            // ============================
            builder.HasMany(w => w.LedgerEntries)
                       .WithOne(le => le.Wallet)
                       .HasForeignKey(le => le.WalletId)
                       .OnDelete(DeleteBehavior.NoAction);

            builder.Navigation(w => w.LedgerEntries)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            // ============================
            // Recharges (Aggregate Child)
            // ============================

            builder.HasMany(w => w.Recharges)
                      .WithOne(r => r.Wallet)
                      .HasForeignKey(r => r.WalletId)
                      .OnDelete(DeleteBehavior.NoAction);
            builder.Navigation(w => w.Recharges)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
