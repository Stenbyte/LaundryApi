

namespace TenantApi.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> FindById(string id);
        Task<int> Create(T entity);
        Task<T?> Find(T entity);
        Task<T?> Update(T entity);
    }
}