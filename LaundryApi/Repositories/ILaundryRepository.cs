
using LaundryApi.Models;
using MongoDB.Driver;

namespace LaundryApi.Repository;

public interface ILaundryRepository
{

    public string TestConnection();
    public string TestPgConnection();
    Task CreateUser(User user);

    Task<User?> FindUserById(string userId);
    Task<User?> FindUserByEmail(string email);
    Task<User?> FindExistingUserWithDbName(User newUser);
    Task<User?> FindUserByRefreshToken(string refreshToken);
    Task UpdateUser(User userToUpdate);

}