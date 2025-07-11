using Microsoft.Extensions.Options;
using MongoDB.Driver;
using LaundryApi.Models;
using MongoDB.Bson;
using LaundryApi.Exceptions;
using LaundryApi.Repository;


namespace LaundryApi.Services
{
    public class LaundryService : ILaundryService
    {
        private readonly ILaundryRepository _repository;

        public LaundryService(ILaundryRepository repository)
        {
            _repository = repository;
        }

        public string TestConnection()
        {
            try
            {
                return _repository.TestConnection();
            }
            catch (CustomException ex)
            {
                throw new CustomException("DataBase connection failed", ex, 500);
            }
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