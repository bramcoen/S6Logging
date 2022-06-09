public class LoggingAction
{
    public LoggingAction(string json, string userId, string details)
    {
        Json = json;
        UserId = userId;
        Details = details;
    }

    public string Json { get; set; }
    public string UserId { get; set; }
    public string Details { get; set; }
}