
using LaundryApi.Models;

namespace LaundryApi.Services;

public interface IBookingService
{
    Task<List<Booking>> GetAll(User user);

    Task<Booking> CreateBooking(Booking newBooking, string dbName);

    Task<Booking> UpdateBooking(Booking existingBooking, string dbName);

    Task<Booking> GetBookingsById(string userId, string dbName);

    Task<Booking> FindByUserAndSlotId(string bookingSlotId, string userId, string dbName);

    Task<Booking> FindBookingsByUserId(string userId, string dbName);

}