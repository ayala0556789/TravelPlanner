using System.Text.Json.Serialization;

namespace TravelPlanner.Infrastructure.ExternalServices.GoogleMaps.Models;

public class GooglePlacesResponse
{
    [JsonPropertyName("results")]
    public List<GoogleResult> Results { get; set; } = new();
}

public class GoogleResult
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("place_id")]
    public string PlaceId { get; set; } = string.Empty;

    [JsonPropertyName("geometry")]
    public Geometry Geometry { get; set; } = new();
}

public class Geometry
{
    [JsonPropertyName("location")]
    public Location Location { get; set; } = new();
}

public class Location
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}