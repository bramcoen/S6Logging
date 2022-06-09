using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

public class LoggingRepository : ILoggingRepository
{
    MongoClient _dbClient;
    IMongoDatabase _mongoDatabase;
    IMongoCollection<LoggingAction> _loggingCollection;
    public LoggingRepository(IConfiguration configuration)
    {
        _dbClient = new MongoClient(configuration["MongoDBConnectionString"]);
        _mongoDatabase = _dbClient.GetDatabase("S6");
        _loggingCollection = _mongoDatabase.GetCollection<LoggingAction>("Logging_Actions");
    }
    public async IEnumerable<LoggingAction> GetForUser(string userId)
    {
        IAsyncCursor<LoggingAction> result = await _loggingCollection.FindAsync<LoggingAction>(i => i.UserId == userId);
        return result.ToEnumerable();
    }
    public async Task StoreLoggingActionAsync(LoggingAction action)
    {
        await _loggingCollection.InsertOneAsync(action);
    }
}