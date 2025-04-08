using Fillow.Models;
using MongoDB.Driver;

namespace Fillow.Services
{
    public class UserService
    {
        private readonly IMongoCollection<UserModel> _users;

        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            var database = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
            _users = database.GetCollection<UserModel>("Accounts");
        }

        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(UserModel user)
        {
            await _users.InsertOneAsync(user);
        }

    }
}
