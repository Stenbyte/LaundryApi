using TenantApi.Models;
using TenantApi.Repository;


namespace TenantApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateUser(string collectionName, User user)
        {
            await _repository.CreateUser(user);
        }

        public async Task<User?> FindUserById(string userId)
        {
            return await _repository.FindUserById(userId);
        }
        public async Task<User?> FindUserByEmail(string email)
        {
            return await _repository.FindUserByEmail(email);
        }
        public async Task<User?> FindExistingUserWithDbName(User newUser)
        {
            return await _repository.FindExistingUserWithDbName(newUser);
        }

        public async Task<User?> FindUserByRefreshToken(string refreshToken)
        {
            return await _repository.FindUserByRefreshToken(refreshToken);
        }

        public async Task UpdateUser(User userToUpdate)
        {
            await _repository.UpdateUser(userToUpdate);
        }
    }
}