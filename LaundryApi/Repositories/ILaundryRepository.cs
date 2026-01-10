
using LaundryApi.Models;
using MongoDB.Driver;

namespace LaundryApi.Repository;

public interface ILaundryRepository
{

    public string TestConnection();
    public string TestPgConnectionWithDbContext();

}