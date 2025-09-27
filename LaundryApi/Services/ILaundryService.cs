using LaundryApi.Models;

namespace LaundryApi.Services;

public interface ILaundryService 
{
    string TestConnection();
    string TestPgConnection();
    string TestPgConnectionWithDbContext();
    Task CreateUser(string collectionName, User user);
    Task<User?> FindUserById(string userId);
    Task<User?> FindUserByEmail(string email);
    Task<User?> FindExistingUserWithDbName(User newUser);
    Task<User?> FindUserByRefreshToken(string refreshToken);
    Task UpdateUser(User userToUpdate);
}