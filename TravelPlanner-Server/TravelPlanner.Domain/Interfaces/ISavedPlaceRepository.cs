using TravelPlanner.Domain.Entities;

namespace TravelPlanner.Domain.Interfaces;

public interface ISavedPlaceRepository
{
    // שליפת מועדפים - מחזירה את כל המקומות שהמשתמש שמר (עם הלב/נעץ)
    Task<IEnumerable<SavedPlace>> GetSavedPlacesByUserIdAsync(int userId);

    // שמירת מקום - מוסיפה מקום חדש לרשימת ה"שמורים" של המשתמש
    Task AddSavedPlaceAsync(SavedPlace place);

    // הסרה מהמועדפים - מחיקת מקום מהרשימה האישית
    Task DeleteSavedPlaceAsync(int placeId,int userId);

    // שמירת שינויים - ביצוע הכתיבה הסופית ל-DB
    Task SaveChangesAsync();
}