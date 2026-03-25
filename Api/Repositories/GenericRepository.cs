

// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Options;
// using MongoDB.Driver;
// using TenantApi.Models;
// 
// namespace TenantApi.Repository
// {
//     public class GenericRepository<T> : IGenericRepository<T> where T : class
//     {
//         protected readonly TenantDbContext _dbContext;
//         protected readonly DbSet<T> _dbSet;
//         public GenericRepository(TenantDbContext dbContext)
//         {
//             _dbContext = dbContext;
//             _dbSet = _dbContext.Set<T>();
// 
//         }
// 
//         public async Task<int> Create(T entity)
//         {
//             _dbSet.Add(entity!);
//            return await _dbContext.SaveChangesAsync();
//         }
// 
//         public async Task<T?> Find(int id)
//         {
//             return await _dbSet.FindAsync(id);
//         }
//     }
// }