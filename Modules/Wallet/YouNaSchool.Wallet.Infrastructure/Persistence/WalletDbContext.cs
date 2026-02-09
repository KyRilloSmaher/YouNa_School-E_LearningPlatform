using Microsoft.EntityFrameworkCore;
using SharedKernel.Application.OUTBOX_PATTERN;
using Wallet.Infrastructure.Persistence.Configurations;
using YouNaSchool.Wallet.Domain.Entities;
using YouNaSchool.Wallet.Infrastructure.Persistence.Configurations;

namespace Wallet.Infrastructure.Persistence;

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
        modelBuilder.ApplyConfiguration(new WalletConfiguration());
        modelBuilder.ApplyConfiguration(new WalletLedgerEntryConfiguration());
        modelBuilder.ApplyConfiguration(new WalletRechargeConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}