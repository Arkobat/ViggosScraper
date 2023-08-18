using Microsoft.EntityFrameworkCore;

namespace ViggosScraper.Database;

public class ViggosDb : DbContext
{
    public DbSet<DbLogoGroup> LogoGroups { get; set; } = null!;
    public DbSet<DbLogo> LogosDates { get; set; } = null!;
    public DbSet<DbUser> Users { get; set; }

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
        });
    }
}