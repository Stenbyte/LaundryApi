using Microsoft.Extensions.Options;
using MongoDB.Driver;
using LaundryBooking.Models;
using MongoDB.Bson;

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
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
        public async Task<List<T>> GetAll<T>(string collectionName)
        {
            // TODO fix later to fetch only bookings for user
            var collection = GetCollection<T>(collectionName);
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task<T?> GetById<T>(string collectionName, string id) where T : IEntity
        {
            var collection = GetCollection<T>(collectionName);
            return await collection.Find(x => x._id == id).FirstOrDefaultAsync();
        }

        public async Task Create<T>(string collectionName, T entity)
        {
            var collection = GetCollection<T>(collectionName);
            await collection.InsertOneAsync(entity);
        }
    }
}