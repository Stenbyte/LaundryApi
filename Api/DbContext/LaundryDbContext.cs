using TenantApi.Models;
using Microsoft.EntityFrameworkCore;

public class TenantDbContext : DbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) { }

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


        // add value converter and conversion for adress e.g. !!!!
        // add value conversion for dates later
        // maybe add owned types for entityt https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities do it later when something arises
        // add encryption Always Encrypted on SQL Server. for value conversion later

        // consider bulk config when completely switched to postgresql
    }
}