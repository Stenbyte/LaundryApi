
using TenantApi.Models;

namespace TenantApi.Repository;

public interface IUserRepository
{
    Task CreateUser(User user);

    Task Create(UserPg user);

    Task<User> FindUserById(string userId);
    Task<UserPg> FindUserByEmail(string email);
    Task<User> FindExistingUserWithDbName(User newUser);
    Task<User> FindUserByRefreshToken(string refreshToken);
    Task UpdateUser(UserPg userToUpdate);

}