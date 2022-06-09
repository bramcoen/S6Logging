public class LoggingAction
{
    public LoggingAction(string json, string userId)
    {
        Json = json;
        UserId = userId;
    }

    public string Json { get; set; }
    public string UserId { get; set; }
    public string Details { get; set; }
}