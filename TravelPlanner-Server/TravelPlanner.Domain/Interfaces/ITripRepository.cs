using TravelPlanner.Domain.Entities;

namespace TravelPlanner.Domain.Interfaces;

public interface ITripRepository
{
    // שליפת טיול עם כל הנקודות שלו - משמש להצגת המסלול המלא על מפת גוגל
    Task<Trip?> GetTripWithPlacesAsync(int tripId, int userId);

    // היסטוריית טיולים - מחזירה את כל הטיולים שבוצעו על ידי משתמש ספציפי
    Task<IEnumerable<Trip>> GetUserTripsAsync(int userId);

    // שמירת טיול חדש - נוצר לאחר שהמשתמש אישר את הצעת ה-AI
    Task AddTripAsync(Trip trip);

    // עדכון סדר הנקודות - תומך בפיצ'ר ה-Drag & Drop; מעדכן את ה-OrderIndex ב-DB
    Task UpdateTripPlacesOrderAsync(int tripId, List<TripPlace> updatedPlaces);

    // שמירת רשימת הציוד - מקבלת מחרוזת JSON שנוצרה על ידי ה-AI ושומרת אותה בטיול
    Task UpdateEquipmentListAsync(int tripId, int userId, string equipmentJson);

    // שליפת רשימת הציוד - מחזירה רק את ה-JSON כדי להציג את ה-Checkboxes באנגולר
    Task<string> GetEquipmentListAsync(int tripId, int userId);

    // מחיקת טיול - מאפשר למשתמש לנקות טיולים ישנים מההיסטוריה
    Task DeleteTripAsync(int tripId,int userId);

    // שמירת שינויים - הפעולה שמבצעת את כל השינויים שנאספו מול מסד הנתונים
    Task SaveChangesAsync();
}