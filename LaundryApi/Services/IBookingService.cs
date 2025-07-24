
using LaundryApi.Models;

namespace LaundryApi.Services;

public interface IBookingService
{
    Task<List<Booking>> GetAllBookingsByBuildingId(User user);
    Task<List<Booking>> GetAllBookingsByMachineId(User user, string machineId);

    Task<Booking> CreateBooking(Booking newBooking, string dbName);

    Task<Booking> UpdateBooking(Booking existingBooking, string dbName);

    Task<Booking> GetBookingsByUserId(string userId, string dbName);

    Task<Booking> FindByUserAndSlotId(string bookingSlotId, string userId, string dbName);

    Task<Booking> FindBookingsByUserId(string userId, string dbName);

    Task<bool> CancelBooking(string userId, string dbName);
    Task<MachineModel> GetMachine(string dbName, string machineId);

    Task<List<MachineModel>> GetAllMachinesByBuildingId(User user);

    Task<MachineModel> CreateMachine(string dbName, MachineModel newMachine);
}