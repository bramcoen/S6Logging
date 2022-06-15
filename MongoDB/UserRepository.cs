using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBRepository
{
    public class UserRepository : IUserRepository
    {

        MongoClient _dbClient;
        IMongoDatabase _mongoDatabase;
        IMongoCollection<User> _usersCollection;
        public UserRepository(IConfiguration configuration)
        {
            _dbClient = new MongoClient(configuration.GetValue<string>("MongoDBConnectionString"));
            _mongoDatabase = _dbClient.GetDatabase("S6");
            _usersCollection = _mongoDatabase.GetCollection<User>("Logging_Users");
        }
       
        public async Task<User> GetById(string id)
        {
            return await _usersCollection.Find(i => i.Id == id).FirstOrDefaultAsync();
        }
        public async Task<User> GetByEmail(string email)
        {
            return await _usersCollection.Find(i => i.Email == email).FirstOrDefaultAsync();
        }
        public async Task<User> GetByUsername(string name)
        {
            return await _usersCollection.Find(i => i.Name == name).FirstOrDefaultAsync();
        }
        public async Task<User> RegisterOrUpdateUser(string name, string userId, string email)
        {
            try
            {
                var user = await GetById(userId);
                if (user == null)
                {
                    user = new User(name,email, userId);
                    await _usersCollection.InsertOneAsync(user);
                    return user;
                }

                var update = Builders<User>.Update.Set(i => i.Name, name);
                var result = await _usersCollection.UpdateOneAsync(i => i.Id == user.Id, update);
                user.Name = name;
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}