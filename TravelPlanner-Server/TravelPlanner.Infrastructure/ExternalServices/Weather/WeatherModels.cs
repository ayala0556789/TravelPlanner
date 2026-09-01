using System.Text.Json.Serialization;

namespace TravelPlanner.Infrastructure.ExternalServices.Weather;

public class WeatherModels
{
    public class WeatherResponse
    {
        [JsonPropertyName("current_weather")]
        public CurrentWeather CurrentWeather { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("weathercode")]
        public int WeatherCode { get; set; }
    }
}