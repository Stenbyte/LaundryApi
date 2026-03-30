using TenantApi.Models;
using Microsoft.EntityFrameworkCore;

public class TenantDbContext : DbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) { }

    public DbSet<UserPg> Users { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Building> Buildings { get; set; }
    public DbSet<UserProperty> UserProperties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Property>(e => {
            e.HasOne(p => p.Building)
             .WithMany(b => b.Units)
             .HasForeignKey(p => p.BuildingId)
             .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<UserProperty>(e => {
            e.HasKey(up => new { up.UserId, up.PropertyId });

            e.HasOne(up => up.User).WithMany(u => u.UserProperty).HasForeignKey(up => up.UserId).OnDelete(DeleteBehavior.Cascade);

            e.HasOne(up => up.Property).WithMany(p => p.UserProperty).HasForeignKey(up => up.PropertyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserPg>(e => {
            e.HasIndex(u => u.Email).IsUnique();
        });


        // add this when completing switch from mongo user class
        // update C# and efCore to LTS

        // add value converter and conversion for adress e.g. !!!!
        // add value conversion for dates later
        // maybe add owned types for entityt https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities do it later when something arises
        // add encryption Always Encrypted on SQL Server. for value conversion later

        // consider bulk config when completely switched to postgresql
    }
}





/// Maybe have it in a future to pass buildingId
/// public class ApplicationDbContext : DbContext
// {
//     private readonly int _currentBuildingId;
// 
// // You inject a service that knows who is logged in
// public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService)
//         : base(options)
//     {
//     _currentBuildingId = tenantService.GetBuildingId();
// }
// 
// public DbSet<LaundryBooking> Bookings { get; set; }
// 
// protected override void OnModelCreating(ModelBuilder modelBuilder)
// {
//     base.OnModelCreating(modelBuilder);
// 
//     // Apply a global filter to any entity implementing ITenantEntity
//     foreach (var entityType in modelBuilder.Model.GetEntityTypes())
//     {
//         if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
//         {
//             modelBuilder.Entity(entityType.ClrType)
//                 .HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
//         }
//     }
// }
// 
// // Helper to create the lambda expression: x => x.BuildingId == _currentBuildingId
// private LambdaExpression ConvertFilterExpression(Type type)
// {
//     var parameter = Expression.Parameter(type, "x");
//     var property = Expression.Property(parameter, nameof(ITenantEntity.BuildingId));
//     var comparison = Expression.Equal(property, Expression.Constant(_currentBuildingId));
//     return Expression.Lambda(comparison, parameter);
// }
// }
/// 