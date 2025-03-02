using Microsoft.Extensions.Options;
using MongoDB.Driver;
using LaundryBooking.Models;
using MongoDB.Bson;

namespace Laundry.Services
{
    public class LaundryService
    {
        private readonly IMongoDatabase _database;

        public LaundryService(IOptions<MongoDBSettings> mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);
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
        public async Task Create<T>(string collectionName, T entity)
        {
            var collection = _database.GetCollection<T>(collectionName);
            await collection.InsertOneAsync(entity);
        }

    }
}