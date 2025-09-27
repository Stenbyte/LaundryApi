using LaundryApi.Models;
using Microsoft.EntityFrameworkCore;

public class LaundryDbContext : DbContext
{
    public LaundryDbContext(DbContextOptions<LaundryDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}