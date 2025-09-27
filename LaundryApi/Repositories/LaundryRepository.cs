
using LaundryApi.Exceptions;
using LaundryApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LaundryApi.Repository;

public class LaundryRepository : ILaundryRepository
{
    private readonly IMongoDatabase _laundryDb;
    private readonly IMongoCollection<User> _userCollection;
    private readonly Npgsql.NpgsqlConnection? _pgConnection;

    private readonly LaundryDbContext _dbContext;

    public LaundryRepository(MongoClient _client, IOptions<MongoDBSettings> mongoSettings, Npgsql.NpgsqlConnection pgConnection, LaundryDbContext dBContext)
    {
        _laundryDb = _client.GetDatabase(mongoSettings.Value.DatabaseName);
        _userCollection = _laundryDb.GetCollection<User>(mongoSettings.Value.UsersCollectionName);

        _pgConnection = pgConnection;

        _dbContext = dBContext;
    }

    public string TestConnection()
    {
        try
        {
            _laundryDb.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            return _laundryDb.DatabaseNamespace.DatabaseName;
        }
        catch (CustomException ex)
        {
            throw new CustomException("DataBase connection failed", ex, 500);
        }
    }

    public string TestPgConnectionWithDbContext()
    {
        try
        {
            var canConnect = _dbContext.Database.CanConnect();
            return canConnect
                ? "✅ Successfully connected to Postgres via DbContext"
                : "❌ Failed to connect to Postgres via DbContext";
        }
        catch (Exception ex)
        {
            throw new CustomException("🍉🍉🍉 Failed to connect to Postgres with DbContext 🍉🍉🍉", ex, 500);
        }
    }


    public string TestPgConnection()
    {
        try
        {
            _pgConnection.Open();
            using var cmd = new Npgsql.NpgsqlCommand("SELECT 1", _pgConnection);
            var result = cmd.ExecuteScalar();
            return $"Test Connection to Postgres: {result}";
        }
        catch (CustomException ex)
        {
            throw new CustomException("🍉🍉🍉Failed to connect to Postgres🍉🍉🍉", ex, 500);
        }
    }

    public async Task CreateUser(User user)
    {
        await _userCollection.InsertOneAsync(user);
    }

    public async Task<User?> FindUserById(string userId)
    {
        var existingUser = await _userCollection.Find(user => user._id == userId).FirstOrDefaultAsync();

        return existingUser;
    }

    public async Task<User?> FindExistingUserWithDbName(User newUser)
    {
        var existingUserWithDbName = await _userCollection.Find(user => user.adress.streetName == newUser.adress.streetName && user.adress.buildingNumber == newUser.adress.buildingNumber).FirstOrDefaultAsync();

        return existingUserWithDbName;
    }
    public async Task<User?> FindUserByRefreshToken(string refreshToken)
    {
        var existingUser = await _userCollection.Find(user => user.refreshToken == refreshToken).FirstOrDefaultAsync();

        return existingUser;
    }

    public async Task<User?> FindUserByEmail(string email)
    {
        var existingUser = await _userCollection.Find(user => user.email == email).FirstOrDefaultAsync();

        return existingUser;
    }

    public async Task UpdateUser(User userToUpdate)
    {
        // revisit if i need return a result ?
        var filter = Builders<User>.Filter.Eq(user => user._id, userToUpdate._id);
        var update = Builders<User>.Update
            .Set(u => u.refreshToken, userToUpdate.refreshToken)
            .Set(u => u.refreshTokenExpiry, userToUpdate.refreshTokenExpiry);

        var updateResult = await _userCollection.UpdateOneAsync(filter, update);

        if (updateResult.ModifiedCount == 0)
        {
            throw new CustomException("User not found or no changes made", null, 400);
        }
        // return updateResult;

    }
}