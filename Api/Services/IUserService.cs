using TenantApi.Models;

namespace TenantApi.Services;

public interface IUserService
{
    Task CreateUser(string collectionName, User user);

    Task Create(UserPg user);
    Task<User?> FindUserById(string userId);
    Task<UserPg?> FindUserByEmail(string email);
    Task<User?> FindExistingUserWithDbName(User newUser);
    Task<UserPg?> FindUserByRefreshToken(string refreshToken);
    Task UpdateUser(UserPg userToUpdate);
}