using Microsoft.Extensions.Options;
using MongoDB.Driver;
using LaundryBooking.Models;

namespace LaundryBooking.Services
{
    public class LaundryService
    {
        private readonly IMongoDatabase _database;

        public LaundryService(IOptions<MongoDBSettings> mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);
            _database = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);
        }
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}