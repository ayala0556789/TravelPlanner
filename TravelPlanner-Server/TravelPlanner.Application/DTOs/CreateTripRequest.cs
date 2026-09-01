using System.Text.Json.Serialization;

namespace TravelPlanner.Application.DTOs;

// אובייקט שמייצג רק את מה שהמשתמש מזין בטופס באנגולר
//public record CreateTripRequest(string Destination, DateTime TravelDate, int DurationInHours);
public class CreateTripRequest
{
    [JsonPropertyName("destination")]
    public string Destination { get; set; } = string.Empty;

    [JsonPropertyName("travelDate")]
    public DateTime TravelDate { get; set; }

    [JsonPropertyName("durationInHours")]
    public int DurationInHours { get; set; }
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
    
    //להוסיף 

    [JsonPropertyName("budget")]
    public string? Budget { get; set; } // Cheap, Moderate, High

    [JsonPropertyName("includeBaby")]
    public bool IncludeBaby { get; set; } // האם התינוק מצטרף הפעם?

    [JsonPropertyName("requiresAccessibility")]
    public bool RequiresAccessibility { get; set; } // דרישת נגישות ספציפית לטיול

    // להוסיף בתוך class CreateTripRequest
[JsonPropertyName("selectedChildrenAges")]
public List<int> SelectedChildrenAges { get; set; } = new(); // הגילאים שהמשתמש סימן ב-V


}