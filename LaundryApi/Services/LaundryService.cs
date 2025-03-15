using Microsoft.Extensions.Options;
using MongoDB.Driver;
using LaundryApi.Models;
using MongoDB.Bson;
using LaundryApi.Controllers;
using LaundryApi.Exceptions;

namespace LaundryApi.Services
{
    public class LaundryService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoClient _mongoClient;
        private readonly IOptions<MongoDBSettings> _mongoSettings;

        public LaundryService(IOptions<MongoDBSettings> mongoSettings)
        {
            _mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);
            _mongoSettings = mongoSettings;
            _database = _mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);
        }

        public IMongoDatabase GetUserDatabase(string dbName)
        {
            return _mongoClient.GetDatabase(dbName);
        }

        public string TestConnection()
        {
            try
            {
                _database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                return _database.DatabaseNamespace.DatabaseName;
            }
            catch (CustomException ex)
            {
                throw new CustomException("DataBase connection failed", ex, 500);
            }
        }
        public async Task CreateUser<T>(string collectionName, T entity)
        {
            var collection = _database.GetCollection<T>(collectionName);
            await collection.InsertOneAsync(entity);
        }

        public async Task<T?> FindUser<T>(T newUser) where T : User
        {
            var collection = _database.GetCollection<T>(_mongoSettings.Value.UsersCollectionName);

            var existingUser = await collection.Find(user => user.Id == newUser.Id).FirstOrDefaultAsync();

            return existingUser;
        }
        public async Task<T?> FindUserByEmail<T>(string email) where T : User
        {
            var collection = _database.GetCollection<T>(_mongoSettings.Value.UsersCollectionName);

            var existingUserWithDbName = await collection.Find(user => user.email == email).FirstOrDefaultAsync();

            return existingUserWithDbName;
        }
        public async Task<T?> FindExistingUserWithDbName<T>(T newUser) where T : User
        {
            var collection = _database.GetCollection<T>(_mongoSettings.Value.UsersCollectionName);

            var existingUserWithDbName = await collection.Find(user => user.adress.streetName == newUser.adress.streetName).FirstOrDefaultAsync();

            return existingUserWithDbName;
        }

        public async Task<T?> FindUserByRefreshToken<T>(string refreshToken) where T : User
        {
            var collection = _database.GetCollection<T>(_mongoSettings.Value.UsersCollectionName);
            var existingUser = await collection.Find(user => user.refreshToken == refreshToken).FirstOrDefaultAsync();

            return existingUser;
        }

        public async Task UpdateUser<T>(T userToUpdate) where T : User
        {
            var collection = _database.GetCollection<T>(_mongoSettings.Value.UsersCollectionName);

            var filter = Builders<T>.Filter.Eq(user => user.Id, userToUpdate.Id);
            var update = Builders<T>.Update
                .Set(u => u.refreshToken, userToUpdate.refreshToken)
                .Set(u => u.refreshTokenExpiry, userToUpdate.refreshTokenExpiry);

            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                throw new CustomException("User not found or no changes made", null, 400);
            }

        }
    }
}