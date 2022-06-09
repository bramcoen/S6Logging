public interface ILoggingRepository
{
    public Task<IEnumerable<LoggingAction>> GetForUser(string userId);
    public Task StoreLoggingActionAsync(LoggingAction action);
}