

using LaundryApi.Models;
using LaundryApi.Repository;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LaundryApi.Repository;

class BookingRepository : IBookingRepository
{

    private readonly MongoClient _mongoclient;
    private readonly IOptions<MongoDBSettings> _mongoSettings;
    private readonly IMongoDatabase _db;
    private readonly IMongoCollection<Booking> _bookingCollection;

    public BookingRepository(IOptions<MongoDBSettings> mongoDbSettings, string dbName) { 
            _mongoclient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            _db = _mongoclient.GetDatabase(dbName);
            _bookingCollection = _db.GetCollection<Booking>("Booking");
    }
}