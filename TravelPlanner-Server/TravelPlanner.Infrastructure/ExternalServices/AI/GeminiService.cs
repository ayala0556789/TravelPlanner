using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TravelPlanner.Domain.DTOs;
using TravelPlanner.Domain.Entities;
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Infrastructure.ExternalServices.AI
    ;

public class GeminiService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string ModelName = "gemini-flash-lite-latest"; // אפשר לעדכן לפי מה שמופיע ב-AI Studio

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
    }

    public async Task<List<TripPlace>> SelectAndEnrichPlacesAsync(
        List<TripPlace> candidatePlaces,
        UserPreferencesDto userPreferences,
        int durationInHours,
        string category,
        DateTime travelDate,
        string budget,
        string weatherCondition
        )
    {
        // Fallback בטיחותי: אם אין בכלל מועמדים, אין מה לעשות
        if (candidatePlaces == null || candidatePlaces.Count == 0)
            return new List<TripPlace>();

        try
        {
            var prompt = BuildPrompt(candidatePlaces, userPreferences, durationInHours, category, travelDate,  budget, weatherCondition);

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent?key={_apiKey}";

            var requestBody = new GeminiRequest
            {
                Contents = new List<GeminiContent>
                {
                    new GeminiContent
                    {
                        Parts = new List<GeminiPart> { new GeminiPart { Text = prompt } }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Gemini API error: {response.StatusCode}");
                return FallbackSelection(candidatePlaces);
            }

            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            var rawText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            if (string.IsNullOrWhiteSpace(rawText))
                return FallbackSelection(candidatePlaces);

            // ניקוי Markdown fences (```json ... ```) שהמודל לפעמים מוסיף
            var cleanJson = rawText.Trim().Replace("```json", "").Replace("```", "").Trim();

            var parsed = JsonSerializer.Deserialize<AiSelectionResult>(cleanJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (parsed?.SelectedPlaces == null || parsed.SelectedPlaces.Count == 0)
                return FallbackSelection(candidatePlaces);

            // מיפוי בין מה שה-AI בחר לבין הנתונים המדויקים שכבר יש לנו מגוגל (RAG!)
            var result = new List<TripPlace>();
            int orderIndex = 0;

            foreach (var selected in parsed.SelectedPlaces)
            {
                var match = candidatePlaces.FirstOrDefault(p => p.GooglePlaceId == selected.GooglePlaceId);
                if (match != null)
                {
                    match.Description = selected.Description;
                    match.OrderIndex = orderIndex++;
                    result.Add(match);
                }
            }

            return result.Count > 0 ? result : FallbackSelection(candidatePlaces);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gemini Service Error: {ex.Message}");
            return FallbackSelection(candidatePlaces);
        }
    }



    private string BuildPrompt(
    List<TripPlace> places, UserPreferencesDto prefs, int hours,
    string category, DateTime date, string budget, string weatherCondition)
    {
        var placesJson = JsonSerializer.Serialize(places.Select(p => new
        {
            p.GooglePlaceId,
            p.PlaceName
        }));

        int suggestedCount = Math.Clamp(hours / 2, 2, Math.Min(5, places.Count));

        var babyNote = prefs.IsTravelingWithBaby
            ? "The user is traveling with a baby - strongly prioritize places close to each other, minimizing travel time between stops, to avoid tiring the baby."
            : "";

        var accessibilityNote = prefs.PreferAccessiblePaths
            ? "The user requires accessible paths (wheelchair / stroller friendly) - avoid places with stairs-only access or rough terrain."
            : "";

        return $@"
You are a professional trip-planning assistant. Below is a list of candidate places in JSON format:
{placesJson}

Trip context:
- Date: {date:yyyy-MM-dd}
- Available time: {hours} hours
- Preferred style: {category}
- Budget: {(budget == "free" ? "Free places only (parks, viewpoints, national sites)" : "Paid attractions are OK")}
- Weather forecast: {weatherCondition}
- Home address: {prefs.HomeAddress}
{babyNote}
{accessibilityNote}

Task:
1. Select approximately {suggestedCount} places from the list that best fit the available time, style and budget.
2. Order them in a geographically logical route, prioritizing minimal travel distance between consecutive stops - not just a logical path, but the most comfortable and least tiring order for the specific travelers described above.
3. Consider the weather: prefer outdoor places on clear days, and indoor/covered places if the forecast is rainy or extreme.
4. For each selected place, write a short, appealing description IN HEBREW (max 20 words), explaining why it fits this trip.

Return ONLY a valid JSON object matching this exact schema, with no extra text:
{{ ""selectedPlaces"": [ {{ ""googlePlaceId"": ""..."", ""description"": ""...בעברית...""}} ] }}";
    }


    //    private string BuildPrompt(
    //        List<TripPlace> places, UserPreferencesDto prefs, int hours, string category, DateTime date,string budget, string weatherCondition)
    //    {
    //        var placesJson = JsonSerializer.Serialize(places.Select(p => new
    //        {
    //            p.GooglePlaceId,
    //            p.PlaceName
    //        }));

    //        // חישוב מספר מקומות מומלץ: בערך מקום אחד לכל 1.5-2 שעות, אבל בין 2 ל-5
    //        int suggestedCount = Math.Clamp(hours / 2, 2, Math.Min(5, places.Count));

    //        var babyNote = prefs.IsTravelingWithBaby
    //            ? "The user is traveling with a baby - prioritize places close to each other geographically and accessible with a stroller."
    //            : "";

    //        return $@"
    //You are a professional trip-planning assistant. Below is a list of candidate places in JSON format:
    //{placesJson}

    //Trip context:
    //- Date: {date:yyyy-MM-dd}
    //- Available time: {hours} hours
    //- Preferred style: {category}
    //-Budget: {(budget == "free" ? "Free places only (parks, viewpoints, national sites)" : "Paid attractions are OK")}
    //- Weather forecast: {weatherCondition}
    //- Home address: {prefs.HomeAddress}
    //{babyNote}

    //Task:
    //1. Select approximately {suggestedCount} places from the list that best fit the available time and style.
    //2. Order them in a geographically logical route (minimize backtracking).
    //3. For each selected place, write a short, appealing description IN HEBREW (max 20 words), explaining why it fits this trip.

    //Return ONLY a valid JSON object matching this exact schema, with no extra text:
    //{{ ""selectedPlaces"": [ {{ ""googlePlaceId"": ""..."", ""description"": ""...בעברית...""}} ] }}";
    //    }

    private List<TripPlace> FallbackSelection(List<TripPlace> candidatePlaces)
    {
        // ברירת מחדל: 4 המקומות הראשונים שגוגל החזיר, בלי שינוי
        return candidatePlaces.Take(4).ToList();
    }
}