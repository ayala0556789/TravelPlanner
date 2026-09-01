namespace TravelPlanner.Domain.Entities;

public class Trip
{
    // מזהה ייחודי לכל טיול שנשמר במסד הנתונים
    public int Id { get; set; }

    // שם העיר או היעד הכללי שהמשתמש בחר
    public string Destination { get; set; }

    // תאריך הטיול - עוזר לנו להיסטוריה ולתכנון עתידי
    public DateTime TravelDate { get; set; }

    // כמה שעות המשתמש הקציב לטיול הזה (כדי שהלו"ז לא יהיה עמוס מדי)
    public int DurationInHours { get; set; }

    // רשימת הציוד והמסלול יישמרו כאן כמחרוזת טקסט ארוכה (במבנה JSON)
    // בשיטה זו אנחנו חוסכים יצירת המון טבלאות קטנות ומורכבות ב-DB
    public string EquipmentList { get; set; }

    // foreign key (מפתח זר) - מקשר את הטיול הזה למשתמש ספציפי
    public int UserId { get; set; }

    // "Navigation Property" - מאפשר לנו בתוך הקוד לגשת ישירות לאובייקט המשתמש של הטיול
    public User User { get; set; }

    // להוסיף בתוך class Trip
    public double? PredictedTemperature { get; set; } // טמפרטורה שנחזתה
    public string? WeatherCondition { get; set; } // תיאור (Sunny, Cloudy וכו')
    public string? BudgetLevel { get; set; } // רמת תקציב שנבחרה
    public bool IncludeBaby { get; set; } // האם התינוק הצטרף ספציפית בטיול זה
    public bool RequiresAccessibility { get; set; } // האם נדרשה נגישות בטיול זה

    public string? SelectedParticipants { get; set; }//כדי שבעתיד כשהמשתמש יסתכל בהיסטוריה, הוא ידע עם מי הוא יצא לטיול ההוא.

    // קישור לרשימת המקומות של הטיול - זה מה שמאפשר את ה-Include
    public ICollection<TripPlace> TripPlaces { get; set; } = new List<TripPlace>();
}