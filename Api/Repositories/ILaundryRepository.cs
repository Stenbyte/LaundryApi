
using TenantApi.Models;
using MongoDB.Driver;

namespace TenantApi.Repository;

public interface ILaundryRepository
{

    public string TestConnection();
    public string TestPgConnectionWithDbContext();

}