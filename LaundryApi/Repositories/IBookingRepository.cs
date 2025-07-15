

using LaundryApi.Models;
using MongoDB.Driver;

namespace LaundryApi.Repository;

public interface IBookingRepository
{
    IMongoCollection<Booking> GetCollection(string dbName);
    Task<List<Booking>> GetAllBookingsByBuildingId(User user);
    Task<Booking> CreateBooking(Booking newBooking, string dbName);

    Task<Booking> UpdateBooking(Booking existingBooking, string dbName);
    Task<Booking> GetBookingsById(string userId, string dbName);

    Task<Booking> FindByUserAndSlotId(string bookingSlotId, string userId, string dbName);
    Task<Booking> FindBookingsByUserId(string userId, string dbName);
}