using Fillow.Models;
using Fillow.Models.partneradmin;
using MongoDB.Bson;
using MongoDB.Driver;

public class JourneyOrderService : IJourneyOrderService
{
	private readonly IMongoCollection<JourneyOrder> _journeyOrders;

	public JourneyOrderService(IConfiguration config)
	{
		var client = new MongoClient(config.GetConnectionString("MongoDb"));
		var database = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
		_journeyOrders = database.GetCollection<JourneyOrder>("JourneyOrder");
	}

	public async Task CreateOrderAsync(JourneyOrder order)
	{
		order.PONumber = await GeneratePoNumberAsync();

        await _journeyOrders.InsertOneAsync(order);
	}

	public async Task<IEnumerable<JourneyOrder>> GetAllOrdersAsync()
	{
		return await _journeyOrders.Find(order => true).ToListAsync();
	}

	public async Task<JourneyOrder> GetOrderByIdAsync(string id)
	{

		return await _journeyOrders.Find(order => order.Id == id).FirstOrDefaultAsync();
	}

	public async Task<bool> DeleteOrderAsync(string id)
	{
	

		var result = await _journeyOrders.DeleteOneAsync(order => order.Id == id);
		return result.DeletedCount > 0;
	}

    public async Task<string> GeneratePoNumberAsync()
    {
        var maxPoNumberDoc = await _journeyOrders.Find(Builders<JourneyOrder>.Filter.Empty)
          .Sort(Builders<JourneyOrder>.Sort.Descending(order => order.PONumber))
          .Limit(1)
          .FirstOrDefaultAsync();


        int nextNumber = 1; // Default value if no PO numbers exist

        if (maxPoNumberDoc != null && int.TryParse(maxPoNumberDoc.PONumber.Substring(2), out int lastNumber))
        {
            nextNumber = lastNumber + 1;
        }
        else
        {
            // If no data exists, start from 1
            nextNumber = 1;
        }

        // Format the new PO number
        return $"PO{nextNumber:D5}"; // Formats to 'PO00001', 'PO00002', etc.
    }
}

