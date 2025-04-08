using Fillow.Models.partneradmin;
using Fillow.Models;

public interface IJourneyOrderService
{
	Task CreateOrderAsync(JourneyOrder order);
	Task<IEnumerable<JourneyOrder>> GetAllOrdersAsync();
	Task<JourneyOrder> GetOrderByIdAsync(string id);
	Task<bool> DeleteOrderAsync(string id);
}
