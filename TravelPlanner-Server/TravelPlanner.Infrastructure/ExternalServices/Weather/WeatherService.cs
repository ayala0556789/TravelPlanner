using System.Net.Http.Json;
using TravelPlanner.Domain.Interfaces;
using static TravelPlanner.Infrastructure.ExternalServices.Weather.WeatherModels;

namespace TravelPlanner.Infrastructure.ExternalServices.Weather;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(double Temp, string Condition)> GetWeatherAsync(double lat, double lon)
    {
        try
        {
            // פנייה ל-API החינמי של Open-Meteo
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true";

            var response = await _httpClient.GetFromJsonAsync<WeatherResponse>(url);

            if (response?.CurrentWeather != null)
            {
                var temp = response.CurrentWeather.Temperature;
                var condition = MapWeatherCode(response.CurrentWeather.WeatherCode);
                return (temp, condition);
            }
        }
        catch (Exception ex)
        {
            // הדפסת שגיאה ללוג (בסביבת פיתוח)
            Console.WriteLine($"Weather Service Error: {ex.Message}");
        }

        // ערכי ברירת מחדל במקרה של תקלה (כדי שהאפליקציה לא תקרוס)
        return (22.0, "Clear");
    }

    private string MapWeatherCode(int code)
    {
        // מיפוי קודים של Open-Meteo למילים פשוטות שה-AI יבין
        return code switch
        {
            0 => "Clear sky",
            1 or 2 or 3 => "Mainly clear or partly cloudy",
            >= 45 and <= 48 => "Foggy",
            >= 51 and <= 55 => "Drizzle",
            >= 61 and <= 67 => "Rainy",
            >= 71 and <= 77 => "Snowy",
            >= 95 => "Thunderstorm",
            _ => "Cloudy"
        };
    }
}