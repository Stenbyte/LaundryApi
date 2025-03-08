using Microsoft.Extensions.Options;
using MongoDB.Driver;
using LaundryApi.Models;
using MongoDB.Bson;

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
            catch (Exception ex)
            {
                throw new Exception("DataBase connection failed", ex);
            }
        }
        public async Task CreateUser<T>(string collectionName, T entity)
        {
            var collection = _database.GetCollection<T>(collectionName);
            await collection.InsertOneAsync(entity);
        }

        public async Task<T?> FindUserWithExisitingDb<T>(T newUser) where T : User
        {
            var collection = _database.GetCollection<T>(_mongoSettings.Value.UsersCollectionName);

            var existingUserWithDbName = await collection.Find(user => user.adress.streetName == newUser.adress.streetName).FirstOrDefaultAsync();

            return existingUserWithDbName;
        }
    }
}