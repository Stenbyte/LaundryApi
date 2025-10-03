using LaundryApi.Models;
using LaundryApi.PostgresModels;
using Microsoft.EntityFrameworkCore;

public class LaundryDbContext : DbContext
{
    public LaundryDbContext(DbContextOptions<LaundryDbContext> options) : base(options) { }

    public DbSet<UserPg> Users { get; set; }
    public DbSet<Property> Property { get; set; }
    public DbSet<UserProperty> UserProperties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProperty>().HasKey(up => new { up.UserId, up.PropertyId });

        modelBuilder.Entity<UserProperty>().HasOne(up => up.User).WithMany(u => u.UserProperties).HasForeignKey(up => up.UserId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserProperty>().HasOne(up => up.Property).WithMany(p => p.UserProperties).HasForeignKey(up => up.PropertyId).OnDelete(DeleteBehavior.Cascade);
    }
}