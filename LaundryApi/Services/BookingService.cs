using LaundryApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LaundryBooking.Services
{
    public class BookingService
    {
        private readonly IMongoCollection<Booking> _bookingsCollection;

        public BookingService(IOptions<MongoDBSettings> mongoDbSettings, String dbName)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var db = mongoClient.GetDatabase(dbName);
            _bookingsCollection = db.GetCollection<Booking>("Booking");
        }
        public async Task<List<Booking>> GetAll()
        {
            return await _bookingsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Booking> CreateBooking(Booking newBooking)
        {

            await _bookingsCollection.InsertOneAsync(newBooking);

            return newBooking;
        }
        public async Task<Booking> UpdateBooking(Booking existingBooking)
        {

            var filter = Builders<Booking>.Filter.Eq(booking => booking.id, existingBooking.id);
            var update = Builders<Booking>.Update.Set(booking => booking.slots, existingBooking.slots).Set(booking => booking.reservationsLeft, existingBooking.reservationsLeft);
            await _bookingsCollection.UpdateOneAsync(filter, update);

            return existingBooking;
        }

        public async Task<Booking> GetBookingsById(string userId)
        {
            return await _bookingsCollection.Find(booking => booking.userId == userId).FirstOrDefaultAsync();
        }


        public async Task<Booking> FindBySlotId(string bookingSlotId)
        {
            return await _bookingsCollection.Find(b => b.slots.Any(slot => slot.id == bookingSlotId)).FirstOrDefaultAsync();
        }
        public async Task<Booking> FindBookingsBuUserId(string userId)
        {
            return await _bookingsCollection.Find(b => b.userId == userId).FirstOrDefaultAsync();
        }


        //         public async Task<T?> GetById<T>(string collectionName, string id) where T : IEntity
        //         {
        //             var collection = GetCollection<T>(collectionName);
        //             return await collection.Find(x => x._id == id).FirstOrDefaultAsync();
        //         }
        // 
        //         public async Task Create<T>(string collectionName, T entity)
        //         {
        //             var collection = GetCollection<T>(collectionName);
        //             await collection.InsertOneAsync(entity);
        //         }
    };
}