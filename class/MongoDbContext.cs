// Data/MongoDbContext.cs
using Fillow.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
        _database = client.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);
    }

    public IMongoCollection<SignUpModel> Accounts
    {
        get
        {
            return _database.GetCollection<SignUpModel>("Accounts");
        }
    }
}
