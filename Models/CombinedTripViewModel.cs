namespace Fillow.Models
{
    public class CombinedTripViewModel
    {
        public TripViewModel TripViewModel { get; set; }
        public IEnumerable<TravelType> TravelTypes { get; set; }
    }
  
}
