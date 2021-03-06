using System.Text.Json.Serialization;

public class LoggingAction
{
    public LoggingAction(string json, string userId, string details)
    {
        Json = json;
        UserId = userId;
        Details = details;
    }
    public LoggingAction(string id, string json, string userId, string details)
    {
        Id = id;
        Json = json;
        UserId = userId;
        Details = details;
    }
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public string Details { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    [JsonIgnore]
    public string Json { get; set; }
}