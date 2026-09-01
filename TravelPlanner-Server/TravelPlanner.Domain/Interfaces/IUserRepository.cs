using TravelPlanner.Domain.Entities;
using TravelPlanner.Domain.DTOs;

namespace TravelPlanner.Domain.Interfaces;

public interface IUserRepository
{
    // שליפת משתמש לפי מזהה ייחודי - משמש להצגת פרופיל או בדיקת הרשאות
    Task<User?> GetByIdAsync(int id);

    // חיפוש לפי ת"ז - קריטי לשלב ה-Login כדי למצוא את המשתמש לפני אימות סיסמה
    Task<User?> GetByIdentityNumberAsync(string identityNumber);

    // שליפת העדפות בלבד (DTO) - זה המידע שנשלח ל-AI כדי שיידע אם יש תינוק או הגבלות מסלול
    Task<UserPreferencesDto> GetUserTravelContextAsync(int userId);

    // הוספת משתמש חדש - משמש את דף ההרשמה (Sign Up)
    Task AddAsync(User user);

    // עדכון פרטים - מאפשר למשתמש לשנות כתובת או העדפות ב"אזור האישי"
    Task UpdateAsync(User user);

    // בדיקת סיסמה - פונקציה שתשווה בין הסיסמה שהוזנה ל-Hash השמור במסד הנתונים
    Task<bool> VerifyPasswordAsync(int userId, string password);
}