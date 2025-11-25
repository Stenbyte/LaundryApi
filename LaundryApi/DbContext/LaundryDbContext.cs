using LaundryApi.PostgresModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LaundryDbContext : DbContext
{
    public LaundryDbContext(DbContextOptions<LaundryDbContext> options) : base(options) { }

    public DbSet<UserPg> Users { get; set; }
    public DbSet<Property> Property { get; set; }
    public DbSet<UserProperty> UserProperties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<UserProperty>(e => {
            e.HasKey(up => new { up.UserId, up.PropertyId });
            e.HasOne(up => up.User).WithMany(u => u.UserProperties).HasForeignKey(up => up.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(up => up.Property).WithMany(p => p.UserProperties).HasForeignKey(up => up.PropertyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserPg>(e => {
            e.HasIndex(u => u.Email).IsUnique();
        });
    }
}