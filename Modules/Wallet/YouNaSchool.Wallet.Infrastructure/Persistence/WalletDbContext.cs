using Microsoft.EntityFrameworkCore;
using SharedKernel.Application.OUTBOX_PATTERN;
using Wallet.Infrastructure.Persistence.Configurations;
using YouNaSchool.Wallet.Domain.Entities;
using YouNaSchool.Wallet.Infrastructure.Persistence.Configurations;

namespace YouNaSchool.Wallet.Infrastructure.Persistence

{

    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options)
            : base(options) { }

        public DbSet<Wallets> Wallets => Set<Wallets>();
        public DbSet<WalletLedgerEntry> WalletLedgerEntries => Set<WalletLedgerEntry>();
        public DbSet<WalletRecharge> WalletRecharges => Set<WalletRecharge>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Wallets");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OutboxMessageConfiguration).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}