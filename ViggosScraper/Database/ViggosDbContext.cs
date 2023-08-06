using Microsoft.EntityFrameworkCore;

namespace ViggosScraper.Database;

public class ViggosDbContext : DbContext
{
    public DbSet<DbUser> Users { get; set; } = null!;
    public DbSet<DbFriendConnection> FriendConnections { get; set; } = null!;

    public ViggosDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            entity.SetTableName("File_" + entity.GetTableName());
        }

        builder.Entity<DbUser>(user =>
        {
            user.HasKey(u => u.Id);
            user.HasIndex(u => u.ProfileId);
            user.HasIndex(u => u.Krus);
        });
        
        
        builder.Entity<DbFriendConnection>(friend =>
        {
            friend.HasKey(f => f.Id);
            
            friend
                .HasOne(f => f.User1)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.User1Id)
                .HasPrincipalKey(u => u.Id);
            
            friend
                .HasOne(f => f.User2)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.User2Id)
                .HasPrincipalKey(u => u.Id);
        });
    }
}