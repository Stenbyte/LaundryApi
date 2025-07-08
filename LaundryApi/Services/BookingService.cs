using LaundryApi.Models;
using LaundryApi.Repository;
using LaundryApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LaundryBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IOptions<MongoDBSettings> _mongoSettings;

        public BookingService(IOptions<MongoDBSettings> mongoDbSettings)
        {
            _mongoSettings = mongoDbSettings;
        }
        public async Task<List<Booking>> GetAll(string dbName)
        {
            var repo = new BookingRepository(_mongoSettings, dbName);
            return await repo.GetAll();
        }

        public async Task<Booking> CreateBooking(Booking newBooking, string dbName)
        {
            var repo = new BookingRepository(_mongoSettings, dbName);
            return await repo.CreateBooking(newBooking);
        }
        public async Task<Booking> UpdateBooking(Booking existingBooking, string dbName)
        {
            var repo = new BookingRepository(_mongoSettings, dbName);
            return await repo.UpdateBooking(existingBooking);
        }

        public async Task<Booking> GetBookingsById(string userId, string dbName)
        {
            var repo = new BookingRepository(_mongoSettings, dbName);
            return await repo.GetBookingsById(userId);
        }


        public async Task<Booking> FindByUserAndSlotId(string bookingSlotId, string userId, string dbName)
        {
            var repo = new BookingRepository(_mongoSettings, dbName);
            return await repo.FindByUserAndSlotId(bookingSlotId, userId);
        }
        public async Task<Booking> FindBookingsByUserId(string userId, string dbName)
        {
            var repo = new BookingRepository(_mongoSettings, dbName);
            return await repo.FindBookingsByUserId(userId);
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