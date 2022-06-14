public interface IUserRepository
{
    public Task<User> RegisterOrUpdateUser(string name, string userId, string email);
    public Task<User> GetByUsername(string name);
    public Task<User> GetById(string id);
    public Task<User> GetByEmail(string email);
}