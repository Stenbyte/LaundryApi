

using LaundryApi.Models;
using LaundryApi.Repository;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LaundryApi.Repository;

class BookingRepository : IBookingRepository
{
    private readonly MongoClient _client;

    public BookingRepository(MongoClient client)
    {
        // inject client to DI and ask about pooling what tf is it
        _client = client;
    }

    public IMongoCollection<Booking> GetCollection(string dbName)
    {
        return _client.GetDatabase(dbName).GetCollection<Booking>("Booking");
    }

    public async Task<List<Booking>> GetAll(string dbName)
    {
        return await GetCollection(dbName).Find(_ => true).ToListAsync();
    }

    public async Task<Booking> CreateBooking(Booking newBooking, string dbName)
    {
        await GetCollection(dbName).InsertOneAsync(newBooking);
        return newBooking;
    }

    public async Task<Booking> UpdateBooking(Booking existingBooking, string dbName)
    {
        var filter = Builders<Booking>.Filter.Eq(booking => booking.id, existingBooking.id);
        var update = Builders<Booking>.Update.Set(booking => booking.slots, existingBooking.slots).
        Set(booking => booking.reservationsLeft, existingBooking.reservationsLeft);

        await GetCollection(dbName).UpdateOneAsync(filter, update);
        return existingBooking;
    }

    public async Task<Booking> GetBookingsById(string userId, string dbName)
    {
        return await GetCollection(dbName).Find(booking => booking.userId == userId).FirstOrDefaultAsync();
    }

    public async Task<Booking> FindByUserAndSlotId(string bookingSlotId, string userId, string dbName)
    {
        return await GetCollection(dbName).Find(b => b.userId == userId && b.slots.Any(slot => slot.id == bookingSlotId)).FirstOrDefaultAsync();
    }

    public async Task<Booking> FindBookingsByUserId(string userId, string dbName)
    {
        return await GetCollection(dbName).Find(b => b.userId == userId).FirstOrDefaultAsync();
    }
}