namespace TravelPlanner.Domain.DTOs;

public class UserPreferencesDto
{

    // כתובת הבית (כדי שה-AI ידע להמליץ על נקודת התחלה/סוף או זמן נסיעה)
    public string HomeAddress { get; set; } = string.Empty;

    public List<int> ChildrenAges { get; set; } = new();      
    public bool PreferAccessiblePaths { get; set; }

    // מחושב: יש תינוק אם יש ילד בגיל 2 ומטה
    public bool IsTravelingWithBaby => ChildrenAges.Any(age => age <= 2);
    // מטרת ה-DTO היא להעביר רק את מה שצריך לתכנון, בלי סיסמאות או ת"ז
}