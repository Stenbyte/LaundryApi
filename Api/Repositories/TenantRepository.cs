
using TenantApi.Exceptions;
using TenantApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TenantApi.Repository;

public class TenantRepository : ITenantRepository
{
    private readonly IMongoDatabase _laundryDb;

    private readonly TenantDbContext _dbContext;

    public TenantRepository(MongoClient _client, IOptions<MongoDBSettings> mongoSettings, TenantDbContext dBContext)
    {
        _laundryDb = _client.GetDatabase(mongoSettings.Value.DatabaseName);

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
}