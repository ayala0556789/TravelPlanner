namespace TravelPlanner.Application.DTOs;

public class TripDto
{
    public int Id { get; set; }
    public string Destination { get; set; } = string.Empty;
    public DateTime TravelDate { get; set; }
    public int DurationInHours { get; set; }

    public string EquipmentList { get; set; } = string.Empty;
    public List<TripPlaceDto> Places { get; set; } = new();

    // להוסיף בתוך class TripDto
    public double? Temperature { get; set; } // יוצג לצד היעד
    public string? WeatherSummary { get; set; } // תיאור מזג האוויר (גשום/שמשי)
}