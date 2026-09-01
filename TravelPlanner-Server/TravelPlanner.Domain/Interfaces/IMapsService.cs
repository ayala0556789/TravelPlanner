using TravelPlanner.Domain.Entities;

namespace TravelPlanner.Domain.Interfaces;

public interface IMapsService
{
    // שליפת פרטים על מקום לפי ה-GooglePlaceId
    Task<List<TripPlace>> GetPlacesForTripAsync(string destination, string category);
}