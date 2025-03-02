// using LaundryBooking.Models;
// using Microsoft.Extensions.Options;
// using MongoDB.Driver;

// namespace LaundryBooking.Services
// {
//     public class BookingService
//     {
//         private readonly IMongoCollection<Booking> _bookingsCollection;

//         public BookingService(IOptions<MongoDBSettings> mongoDbSettings)
//         {
//             var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
//             var mongoDataBase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
//             _bookingsCollection = mongoDataBase.GetCollection<Booking>(mongoDbSettings.Value.BookingsCollectionName);
//         }

//         // public async Task<List<Booking>> GetBookings() => await _bookingsCollection.Find(_ => true).ToListAsync();

//         // public async Task<Booking?> GetBookingById(string id) => await _bookingsCollection.Find(x => x._id == id).FirstOrDefaultAsync();

//         // public async Task CreateBooking(Booking newBooking) => await _bookingsCollection.InsertOneAsync(newBooking);
//         public IMongoCollection<T> GetCollection<T>(string collectionName)
//         {
//             return _database.GetCollection<T>(collectionName);
//         }
//         public async Task<List<T>> GetAll<T>(string collectionName)
//         {
//             // TODO fix later to fetch only bookings for user
//             var collection = GetCollection<T>(collectionName);
//             return await collection.Find(_ => true).ToListAsync();
//         }

//         public async Task<T?> GetById<T>(string collectionName, string id) where T : IEntity
//         {
//             var collection = GetCollection<T>(collectionName);
//             return await collection.Find(x => x._id == id).FirstOrDefaultAsync();
//         }

//         public async Task Create<T>(string collectionName, T entity)
//         {
//             var collection = GetCollection<T>(collectionName);
//             await collection.InsertOneAsync(entity);
//         }
//     }
// }