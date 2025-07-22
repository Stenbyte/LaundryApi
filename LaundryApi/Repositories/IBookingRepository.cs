

using LaundryApi.Models;
using MongoDB.Driver;

namespace LaundryApi.Repository;

public interface IBookingRepository
{
    IMongoCollection<T> GetCollection<T>(string dbName, string collectionName = "Booking");
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
}