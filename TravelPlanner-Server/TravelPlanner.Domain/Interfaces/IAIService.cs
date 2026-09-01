// namespace TravelPlanner.Domain.Interfaces;

// public interface IAIService
// {
//     // פונקציה שתקבל את רשימת המקומות והעדפות המשתמש ותחזיר תיאור מסלול
//     Task<string> GenerateRouteDescriptionAsync(string userPreferences, List<string> placeNames);
// }

// שלב 1 — חיבור ה-AI (RAG Flow)
// נעדכן את הזרימה כך: Google מחזיר מועמדים → אנחנו שולחים אותם ל-Gemini עם פרופיל המשתמש → Gemini בוחר, מסדר ומסביר.
// קובץ 1: IAIService.cs
// מיקום: TravelPlanner.Domain → Interfaces → IAIService.cs (קובץ קיים, נעדכן את החתימה)
// למה: זה ה"חוזה" – ה-Domain לא צריך לדעת שמדובר ב-Gemini, רק שיש שירות שמקבל מועמדים והעדפות ומחזיר רשימה סופית של TripPlace מסוננת, מסודרת ומוסברת.
//csharp
using TravelPlanner.Domain.DTOs;
using TravelPlanner.Domain.Entities;

namespace TravelPlanner.Domain.Interfaces;

public interface IAIService
{
    Task<List<TripPlace>> SelectAndEnrichPlacesAsync(
        List<TripPlace> candidatePlaces,
        UserPreferencesDto userPreferences,
        int durationInHours,
        string category,
        DateTime travelDate,
        string budget,
        string weatherCondition
        );
}