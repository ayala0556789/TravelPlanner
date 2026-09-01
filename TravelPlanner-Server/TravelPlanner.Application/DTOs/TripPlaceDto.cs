namespace TravelPlanner.Application.DTOs;

public class TripPlaceDto
{
    public int Id { get; set; }
    public string PlaceName { get; set; } = string.Empty;
    public string GooglePlaceId { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int OrderIndex { get; set; }
    public string Description { get; set; } = string.Empty;
}
