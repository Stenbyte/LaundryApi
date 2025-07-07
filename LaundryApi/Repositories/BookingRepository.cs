

using LaundryApi.Models;
using LaundryApi.Repository;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LaundryApi.Repository;

class BookingRepository : IBookingRepository
{

    private readonly MongoClient _mongoclient;
    private readonly IOptions<MongoDBSettings> _mongoSettings;
    private readonly IMongoDatabase _db;
    private readonly IMongoCollection<Booking> _bookingCollection;

    public BookingRepository(IOptions<MongoDBSettings> mongoDbSettings, string dbName)
    {
        _mongoclient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        _db = _mongoclient.GetDatabase(dbName);
        _bookingCollection = _db.GetCollection<Booking>("Booking");
    }

    public async Task<List<Booking>> GetAll()
    {
        return await _bookingCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Booking> CreateBooking(Booking newBooking)
    {
        await _bookingCollection.InsertOneAsync(newBooking);
        return newBooking;
    }

    public async Task<Booking> UpdateBooking(Booking existingBooking)
    {
        var filter = Builders<Booking>.Filter.Eq(booking => booking.id, existingBooking.id);
        var update = Builders<Booking>.Update.Set(booking => booking.slots, existingBooking.slots).
        Set(booking => booking.reservationsLeft, existingBooking.reservationsLeft);

        await _bookingCollection.UpdateOneAsync(filter, update);
        return existingBooking;
    }

    public async Task<Booking> GetBookingsById(string userId)
    {
        return await _bookingCollection.Find(booking => booking.userId == userId).FirstOrDefaultAsync();
    }

    public async Task<Booking> FindByUserAndSlotId(string bookingSlotId, string userId)
    {
        return await _bookingCollection.Find(b => b.userId == userId && b.slots.Any(slot => slot.id == bookingSlotId)).FirstOrDefaultAsync();
    }

    public async Task<Booking> FindBookingsByUserId(string userId)
    {
        return await _bookingCollection.Find(b => b.userId == userId).FirstOrDefaultAsync();
    }
}