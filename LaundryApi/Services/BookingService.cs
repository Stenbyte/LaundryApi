using LaundryBooking.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LaundryBookings.Services
{
    public class BookingService
    {
        private readonly IMongoCollection<Booking> _bookingsCollection;

        public BookingService(IOptions<MongoDBSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDataBase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _bookingsCollection = mongoDataBase.GetCollection<Booking>(mongoDbSettings.Value.BookingsCollectionName);
        }

        public async Task<List<Booking>> GetBookings() => await _bookingsCollection.Find(_ => true).ToListAsync();

        public async Task<Booking?> GetBookingById(string id) => await _bookingsCollection.Find(x => x._id == id).FirstOrDefaultAsync();
    }
}