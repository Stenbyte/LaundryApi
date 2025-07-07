using LaundryApi.Models;
using LaundryApi.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LaundryBooking.Services
{
    public class BookingService
    {
        private readonly IMongoCollection<Booking> _bookingsCollection;
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IOptions<MongoDBSettings> mongoDbSettings, String dbName)
        {
            _bookingRepository = new BookingRepository(mongoDbSettings, dbName);
        }
        public async Task<List<Booking>> GetAll()
        {
            return await _bookingRepository.GetAll();
        }

        public async Task<Booking> CreateBooking(Booking newBooking)
        {
            return await _bookingRepository.CreateBooking(newBooking);
        }
        public async Task<Booking> UpdateBooking(Booking existingBooking)
        {
            return await _bookingRepository.UpdateBooking(existingBooking);
        }

        public async Task<Booking> GetBookingsById(string userId)
        {
            return await _bookingRepository.GetBookingsById(userId);
        }


        public async Task<Booking> FindByUserAndSlotId(string bookingSlotId, string userId)
        {
            return await _bookingRepository.FindByUserAndSlotId(bookingSlotId, userId);
        }
        public async Task<Booking> FindBookingsByUserId(string userId)
        {
            return await _bookingRepository.FindBookingsByUserId(userId);
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