namespace TravelPlanner.Application.DTOs;

public class SavedPlaceDto
{
    public int Id { get; set; }
    public string PlaceName { get; set; } = string.Empty;
    public string GooglePlaceId { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime SavedAt { get; set; }

   
}
