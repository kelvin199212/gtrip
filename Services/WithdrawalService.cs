using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Fillow.Models.partneradmin;
using Microsoft.AspNetCore.Mvc;

namespace Fillow.Services
{
    public class WithdrawalService : IWithdrawalService
    {
        private readonly IMongoCollection<WithdrawalRequestModel> _withdrawalRecordsCollection;

        public WithdrawalService(IConfiguration config)
        {
            // Get MongoDB connection string from configuration
            var client = new MongoClient(config.GetConnectionString("MongoDb"));

            // Get the database name from the configuration
            var database = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);

            // Get the collection ("Withdrawal" collection) from the database
            _withdrawalRecordsCollection = database.GetCollection<WithdrawalRequestModel>("Withdrawal");
        }

        public List<WithdrawalRequestModel> GetWithdrawalRecords(string userId)
        {
            // Fetch records from MongoDB collection based on userId
            return _withdrawalRecordsCollection
                .Find(record => record.UserId == userId)
                .ToList();
        }

    
        public bool ProcessWithdrawalRequest(WithdrawalRequestModel model)
        {
            try
            {
               
            
                // Insert the new withdrawal record into the collection
                _withdrawalRecordsCollection.InsertOne(model);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
