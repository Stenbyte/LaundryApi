
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

    private readonly TenantDbContext _dbContext;

    public UserRepository(MongoClient _client, IOptions<MongoDBSettings> mongoSettings, TenantDbContext dbContext)
    {
        _laundryDb = _client.GetDatabase(mongoSettings.Value.DatabaseName);
        _userCollection = _laundryDb.GetCollection<User>(mongoSettings.Value.UsersCollectionName);

        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }


    public async Task CreateUser(User user)
    {
        await _userCollection.InsertOneAsync(user);
    }

    public async Task Create(UserPg user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> FindUserById(string userId)
    {
        var existingUser = await _userCollection.Find(user => user._id == userId).FirstOrDefaultAsync();

        return existingUser;
    }

    public async Task<User> FindExistingUserWithDbName(User newUser)
    {
        var existingUserWithDbName = await _userCollection.Find(user => user.adress.streetName == newUser.adress.streetName && user.adress.buildingNumber == newUser.adress.buildingNumber).FirstOrDefaultAsync();

        return existingUserWithDbName;
    }
    public async Task<User> FindUserByRefreshToken(string refreshToken)
    {
        var existingUser = await _userCollection.Find(user => user.refreshToken == refreshToken).FirstOrDefaultAsync();

        return existingUser;
    }

    public async Task<UserPg> FindUserByEmail(string? email)
    {

        if (email == null || email.Length == 0)
        {
            throw new CustomException("Email is required", null, 400);
        }

        UserPg? existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u!.Email! == email);

        if (existingUser is null)
        {
            throw new CustomException("User not found", null, 404);
        }
        return existingUser!;
    }

    public async Task UpdateUser(UserPg userToUpdate)
    {
        UserPg? user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == userToUpdate.Id);

        if (user == null)
        {
            throw new CustomException("User not found", null, 400);
        }

        user.refreshToken = userToUpdate.refreshToken;
        user.refreshTokenExpiry = userToUpdate.refreshTokenExpiry;

        await _dbContext.SaveChangesAsync();
    }
}