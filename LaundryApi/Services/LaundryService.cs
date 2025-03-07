using Microsoft.Extensions.Options;
using MongoDB.Driver;
using LaundryApi.Models;
using MongoDB.Bson;

namespace LaundryApi.Services
{
    public class LaundryService
    {
        private readonly IMongoDatabase _database;
        private readonly IOptions<MongoDBSettings> _mongoSettings;

        public LaundryService(IOptions<MongoDBSettings> mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);
            _mongoSettings = mongoSettings;
            _database = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);
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

        public async Task<T?> FindUserWithExisitingDb<T>(T newUser) where T : SignUpUser
        {
            var collection = _database.GetCollection<T>(_mongoSettings.Value.UsersCollectionName);

            var existingDbName = await collection.Find(user => user.adress.streetName == newUser.adress.streetName).FirstOrDefaultAsync();

            return existingDbName;
        }
    }
}