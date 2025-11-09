using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Transactions.DAL;

public class TransactionsDataContext(DbContextOptions<TransactionsDataContext> options) : DbContext(options)
{
    public DbSet<Consent> Consents { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<CurrencyCode>();
        base.OnModelCreating(modelBuilder);
    }
}