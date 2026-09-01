namespace TravelPlanner.Domain.Interfaces;

public interface IWeatherService
{
       Task<(double Temp, string Condition)> GetWeatherAsync(double lat, double lon);
}