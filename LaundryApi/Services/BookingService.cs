using LaundryApi.Models;
using LaundryApi.Repository;
using LaundryApi.Services;

namespace LaundryBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        public async Task<List<Booking>> GetAllBookingsByBuildingId(User user)
        {
            return await _bookingRepository.GetAllBookingsByBuildingId(user);
        }
        public async Task<List<Booking>> GetAllBookingsByMachineId(User user, string machineId)
        {
            return await _bookingRepository.GetAllBookingsByMachineId(user, machineId);
        }

        public async Task<Booking> CreateBooking(Booking newBooking, string dbName)
        {
            return await _bookingRepository.CreateBooking(newBooking, dbName);
        }
        public async Task<Booking> UpdateBooking(Booking existingBooking, string dbName)
        {
            return await _bookingRepository.UpdateBooking(existingBooking, dbName);
        }

        public async Task<Booking> GetBookingsByUserId(string userId, string dbName)
        {
            return await _bookingRepository.GetBookingsByUserId(userId, dbName);
        }


        public async Task<Booking> FindByUserAndSlotId(string bookingSlotId, string userId, string dbName)
        {
            return await _bookingRepository.FindByUserAndSlotId(bookingSlotId, userId, dbName);
        }
        public async Task<Booking> FindBookingsByUserId(string userId, string dbName)
        {
            return await _bookingRepository.FindBookingsByUserId(userId, dbName);
        }

        public async Task<bool> CancelBooking(string userId, string dbName)
        {
            return await _bookingRepository.CancelBooking(userId, dbName);
        }

        public async Task<MachineModel> GetMachine(string dbName, string machineId)
        {
            return await _bookingRepository.GetMachine(dbName, machineId);
        }

        public async Task<List<MachineModel>> GetAllMachinesByBuildingId(User user)
        {
            return await _bookingRepository.GetAllMachinesByBuildingId(user);
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