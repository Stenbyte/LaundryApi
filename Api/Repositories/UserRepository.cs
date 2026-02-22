
using TenantApi.Exceptions;
using TenantApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TenantApi.Repository;

public class UserRepository : IUserRepository
{
    private readonly IMongoDatabase _laundryDb;
    private readonly IMongoCollection<User> _userCollection;

    // private readonly TenantDbContext _dbContext;

    // public LaundryRepository(MongoClient _client, IOptions<MongoDBSettings> mongoSettings)
    // {
    //     _laundryDb = _client.GetDatabase(mongoSettings.Value.DatabaseName);
    //     _userCollection = _laundryDb.GetCollection<User>(mongoSettings.Value.UsersCollectionName);
    // }
    public UserRepository(MongoClient _client, IOptions<MongoDBSettings> mongoSettings, TenantDbContext dBContext)
    {
        _laundryDb = _client.GetDatabase(mongoSettings.Value.DatabaseName);
        _userCollection = _laundryDb.GetCollection<User>(mongoSettings.Value.UsersCollectionName);

        // _dbContext = dBContext;
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