using Microsoft.EntityFrameworkCore;

namespace ViggosScraper.Database;

public class ViggosDb : DbContext
{
    public DbSet<DbLogoGroup> LogoGroups { get; set; } = null!;
    public DbSet<DbLogo> LogosDates { get; set; } = null!;
    public DbSet<DbUser> Users { get; set; } = null!;
    public DbSet<BeerPongBattle> BeerPongBattles { get; set; } = null!;
    public DbSet<BattleResult> BattleResults { get; set; } = null!;

    public ViggosDb(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<DbLogoGroup>(logo =>
        {
            logo.HasKey(t => t.Id);
            logo.HasIndex(f => f.Symbol).IsUnique();
            logo.HasMany(l => l.Dates)
                .WithOne(d => d.Group)
                .HasForeignKey(d => d.GroupId)
                .HasPrincipalKey(d => d.Id)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<DbUser>(user =>
        {
            user.HasIndex(u => u.ProfileId).IsUnique();
            user.HasMany(u => u.Datoer)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId);

            user.HasMany(u => u.Permissions)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .HasPrincipalKey(u => u.Id);
        });

        builder.Entity<BeerPongBattle>(battle =>
        {
            battle.HasKey(b => b.BattleId);
            battle.HasMany(b => b.Results)
                .WithOne(r => r.Battle)
                .HasForeignKey(r => r.BattleId)
                .HasPrincipalKey(b => b.BattleId);
        });

        builder.Entity<BattleResult>(result =>
        {
            result.HasKey(r => r.ResultId);
            result.HasOne(r => r.User)
                .WithMany(u => u.Battles)
                .HasForeignKey(r => r.UserId)
                .HasPrincipalKey(u => u.Id);
        });
    }
}