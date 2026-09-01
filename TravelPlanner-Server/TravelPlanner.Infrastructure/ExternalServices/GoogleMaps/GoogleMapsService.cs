using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using TravelPlanner.Domain.Entities;
using TravelPlanner.Domain.Interfaces;
using TravelPlanner.Infrastructure.ExternalServices.GoogleMaps.Models;
using System.Text.Json.Serialization;

namespace TravelPlanner.Infrastructure.ExternalServices.GoogleMaps;

public class GoogleMapsService : IMapsService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GoogleMapsService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        // שליפת המפתח מה-appsettings.json שעדכנת
        _apiKey = configuration["GoogleMaps:ApiKey"];
    }

    public async Task<List<TripPlace>> GetPlacesForTripAsync(string destination, string category)
    {

        // 1. בניית שאילתת החיפוש בהתאם לקטגוריה
        string searchQuery;

        string strictDestination = $"{destination} City, Israel";

        // בדיקה אם נבחר 'כללי' או שלא נבחרה קטגוריה
        if (string.IsNullOrWhiteSpace(category) || category.ToLower() == "general" || category == "כללי")
        {
            // אם כללי - נחפש אטרקציות תיירותיות כלליות סביב המרכז
            searchQuery = $"top tourist attractions in {strictDestination}";
            //searchQuery = $"points of interest and parks inside {strictDestination}";
        }
        else
        {
            // אם נבחרה קטגוריה (למשל history) - נחפש אותה סביב המרכז
            searchQuery = $"{category} attractions in {strictDestination}";
        }

        // קריאה ל-Google Text Search כדי למצוא אטרקציות מובילות ביעד
        //var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query=top+attractions+in+{destination}&key={_apiKey}";
       
        // הוספת "near center of" עוזרת לגוגל להביא מקומות קרובים אחד לשני ולא מפוזרים
        var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(searchQuery)}&language=he&key={_apiKey}";

        try 
        {
            var response = await _httpClient.GetFromJsonAsync<GooglePlacesResponse>(url);
            
            if (response?.Results == null || !response.Results.Any()) {
                Console.WriteLine("there is no place!!");
                return new List<TripPlace>();
            }

            return response.Results.Take(5).Select((res, index) => new TripPlace
            {
                PlaceName = res.Name,
                GooglePlaceId = res.PlaceId,
                Latitude = res.Geometry.Location.Lat,
                Longitude = res.Geometry.Location.Lng,
                OrderIndex = index,
                Description = $"אטרקציה מומלצת ב{destination}"
            }).ToList();
        }
        catch (Exception ex)
        {
            // אם יש שגיאה (למשל אין אינטרנט), מחזירים רשימה ריקה כדי שהאפליקציה לא תקרוס
            Console.WriteLine($"Error calling Google Maps: {ex.Message}");
            return new List<TripPlace>();
        }
    }

    
}