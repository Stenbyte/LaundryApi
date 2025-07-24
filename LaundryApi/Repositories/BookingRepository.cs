using System.Threading.Tasks;
using LaundryApi.Models;
using MongoDB.Driver;

namespace LaundryApi.Repository;

class BookingRepository : IBookingRepository
{
    private readonly MongoClient _client;

    public BookingRepository(MongoClient client)
    {
        _client = client;
    }
    public IMongoCollection<T> GetCollection<T>(string dbName, string collectionName = "Booking")
    {
        return _client.GetDatabase(dbName).GetCollection<T>(collectionName);
    }
    public async Task<List<Booking>> GetAllBookingsByBuildingId(User user)
    {
        return await GetCollection<Booking>(user.dbName).Find(bookings => user.adress.id == bookings.buildingId).ToListAsync();
    }
    public async Task<List<Booking>> GetAllBookingsByMachineId(User user, string machineId)
    {
        var filter = Builders<Booking>.Filter.And(
            Builders<Booking>.Filter.Eq(x => x.machineId, machineId),
            Builders<Booking>.Filter.Eq(x => x.userId, user.id)

        );
        return await GetCollection<Booking>(user.dbName).Find(filter).ToListAsync();
    }

    public async Task<Booking> CreateBooking(Booking newBooking, string dbName)
    {
        await GetCollection<Booking>(dbName).InsertOneAsync(newBooking);
        return newBooking;
    }

    public async Task<Booking> UpdateBooking(Booking existingBooking, string dbName)
    {
        var filter = Builders<Booking>.Filter.Eq(booking => booking.id, existingBooking.id);
        var update = Builders<Booking>.Update.Set(booking => booking.slots, existingBooking.slots).
        Set(booking => booking.reservationsLeft, existingBooking.reservationsLeft).Set(booking => booking.startTime, existingBooking.startTime);

        await GetCollection<Booking>(dbName).UpdateOneAsync(filter, update);
        return existingBooking;
    }

    public async Task<Booking> GetBookingsByUserId(string userId, string dbName)
    {
        return await GetCollection<Booking>(dbName).Find(booking => booking.userId == userId).FirstOrDefaultAsync();
    }

    public async Task<Booking> FindByUserAndSlotId(string bookingSlotId, string userId, string dbName)
    {
        return await GetCollection<Booking>(dbName).Find(b => b.userId == userId && b.slots.Any(slot => slot.id == bookingSlotId)).FirstOrDefaultAsync();
    }

    public async Task<Booking> FindBookingsByUserId(string userId, string dbName)
    {
        return await GetCollection<Booking>(dbName).Find(b => b.userId == userId).FirstOrDefaultAsync();
    }

    public async Task<bool> CancelBooking(string userId, string dbName)
    {
        var result = await GetCollection<Booking>(dbName).DeleteOneAsync(b => b.userId == userId);
        return result.DeletedCount > 0;
    }

    public async Task<MachineModel> GetMachine(string dbName, string machineId)
    {
        return await GetCollection<MachineModel>(dbName, "Machine").Find(machine => machine.id == machineId).FirstOrDefaultAsync();
    }

    public async Task<List<MachineModel>> GetAllMachinesByBuildingId(User user)
    {
        return await GetCollection<MachineModel>(user.dbName, "Machine").Find(x => x.buildingId == user.adress.id).ToListAsync();
    }

    public async Task<MachineModel> CreateMachine(string dbName, MachineModel newMachine)
    {
        await GetCollection<MachineModel>(dbName, "Machine").InsertOneAsync(newMachine);
        return newMachine;
    }
}