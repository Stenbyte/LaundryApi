

using LaundryApi.Models;

namespace LaundryApi.Repository;

public interface IBookingRepository
{
    Task<List<Booking>> GetAll();
    Task<Booking> CreateBooking(Booking newBooking);

    Task<Booking> UpdateBooking(Booking existingBooking);
    Task<Booking> GetBookingsById(string userId);

    Task<Booking> FindByUserAndSlotId(string bookingSlotId, string userId);
    Task<Booking> FindBookingsByUserId(string userId);
}