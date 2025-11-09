using Labubu.Main.DAL;
using Microsoft.EntityFrameworkCore;

namespace Labubu.Main.Data;

public class MainContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<DAL.Labubu> Labubus => Set<DAL.Labubu>();
    public DbSet<Clothes> Clothes => Set<Clothes>();
    public DbSet<LabubuClothes> LabubuClothes => Set<LabubuClothes>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<AchievementProgress> AchievementProgresses => Set<AchievementProgress>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();
    public DbSet<BattlePass> BattlePasses => Set<BattlePass>();
    public DbSet<BattlePassProgress> BattlePassProgresses => Set<BattlePassProgress>();
    public DbSet<BattlePassReward> BattlePassRewards => Set<BattlePassReward>();
    public DbSet<BattlePassPurchase> BattlePassPurchases => Set<BattlePassPurchase>();
    public DbSet<BattlePassRewardClaim> BattlePassRewardClaims => Set<BattlePassRewardClaim>();

    public MainContext(DbContextOptions<MainContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.Labubu)
            .WithOne(l => l.User)
            .HasForeignKey<DAL.Labubu>(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LabubuClothes>()
            .HasKey(lc => new { lc.LabubuId, lc.ClothesId });

        modelBuilder.Entity<LabubuClothes>()
            .HasOne(lc => lc.Labubu)
            .WithMany(l => l.LabubuClothes)
            .HasForeignKey(lc => lc.LabubuId);

        modelBuilder.Entity<LabubuClothes>()
            .HasOne(lc => lc.Clothes)
            .WithMany()
            .HasForeignKey(lc => lc.ClothesId);

        modelBuilder.Entity<AchievementProgress>()
            .HasOne(ap => ap.Achievement)
            .WithMany(a => a.AchievementProgresses)
            .HasForeignKey(ap => ap.AchievementId);

        modelBuilder.Entity<AchievementProgress>()
            .HasOne(ap => ap.Labubu)
            .WithMany(l => l.AchievementProgresses)
            .HasForeignKey(ap => ap.LabubuId);

        modelBuilder.Entity<GameSession>()
            .HasOne(gs => gs.Game)
            .WithMany(g => g.GameSessions)
            .HasForeignKey(gs => gs.GameId);

        modelBuilder.Entity<GameSession>()
            .HasOne(gs => gs.Labubu)
            .WithMany(l => l.GameSessions)
            .HasForeignKey(gs => gs.LabubuId);

        modelBuilder.Entity<BattlePassProgress>()
            .HasOne(bpp => bpp.BattlePass)
            .WithMany(bp => bp.Progresses)
            .HasForeignKey(bpp => bpp.BattlePassId);

        modelBuilder.Entity<BattlePassProgress>()
            .HasOne(bpp => bpp.Labubu)
            .WithMany(l => l.BattlePassProgresses)
            .HasForeignKey(bpp => bpp.LabubuId);

        modelBuilder.Entity<BattlePassReward>()
            .HasOne(bpr => bpr.BattlePass)
            .WithMany(bp => bp.Rewards)
            .HasForeignKey(bpr => bpr.BattlePassId);

        modelBuilder.Entity<BattlePassReward>()
            .HasOne(bpr => bpr.Clothes)
            .WithMany()
            .HasForeignKey(bpr => bpr.ClothesId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<BattlePassPurchase>()
            .HasOne(bpu => bpu.BattlePass)
            .WithMany(bp => bp.Purchases)
            .HasForeignKey(bpu => bpu.BattlePassId);

        modelBuilder.Entity<BattlePassPurchase>()
            .HasOne(bpu => bpu.Labubu)
            .WithMany(l => l.BattlePassPurchases)
            .HasForeignKey(bpu => bpu.LabubuId);

        modelBuilder.Entity<BattlePassRewardClaim>()
            .HasOne(bprc => bprc.BattlePassReward)
            .WithMany(r => r.Claims)
            .HasForeignKey(bprc => bprc.BattlePassRewardId);

        modelBuilder.Entity<BattlePassRewardClaim>()
            .HasOne(bprc => bprc.Labubu)
            .WithMany(l => l.BattlePassRewardClaims)
            .HasForeignKey(bprc => bprc.LabubuId);

        modelBuilder.Entity<BattlePassProgress>()
            .HasIndex(bpp => new { bpp.BattlePassId, bpp.LabubuId })
            .IsUnique();

        modelBuilder.Entity<BattlePassPurchase>()
            .HasIndex(bpu => new { bpu.BattlePassId, bpu.LabubuId })
            .IsUnique();

        modelBuilder.Entity<BattlePassRewardClaim>()
            .HasIndex(bprc => new { bprc.BattlePassRewardId, bprc.LabubuId })
            .IsUnique();
    }
}