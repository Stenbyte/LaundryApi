
using LaundryApi.Models;
using MongoDB.Driver;

namespace LaundryApi.Repository;

public interface ILaundryRepository
{

    Task TestConnection(IMongoDatabase _dataBase);
    Task CreateUser<User>(string collectionName, User user);

    Task<User> FindUserById(string userId);
    Task<User> FindUserByEmail(string email);
    Task<User> FindUserExistingUserWithDbName(User user);
    Task<User> FindUserByRefreshToken(string refreshToken);
    Task<User> UpdateUser<User>(User user);

}