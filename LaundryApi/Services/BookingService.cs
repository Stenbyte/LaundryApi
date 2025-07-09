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
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        public async Task<List<Booking>> GetAll(string dbName)
        {
            return await _bookingRepository.GetAll(dbName);
        }

        public async Task<Booking> CreateBooking(Booking newBooking, string dbName)
        {
            return await _bookingRepository.CreateBooking(newBooking, dbName);
        }
        public async Task<Booking> UpdateBooking(Booking existingBooking, string dbName)
        {
            return await _bookingRepository.UpdateBooking(existingBooking, dbName);
        }

        public async Task<Booking> GetBookingsById(string userId, string dbName)
        {
            return await _bookingRepository.GetBookingsById(userId, dbName);
        }


        public async Task<Booking> FindByUserAndSlotId(string bookingSlotId, string userId, string dbName)
        {
            return await _bookingRepository.FindByUserAndSlotId(bookingSlotId, userId, dbName);
        }
        public async Task<Booking> FindBookingsByUserId(string userId, string dbName)
        {
            return await _bookingRepository.FindBookingsByUserId(userId, dbName);
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