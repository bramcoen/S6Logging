using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class User
{
    public User()
    {

    }
    public User(string name, string email, string id)
    {
        Name = name;
        Email = email;
        Id = id;
    }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Id { get; set; }
}