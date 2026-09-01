namespace TravelPlanner.Domain.Entities;

public class SavedPlace
{
    public int Id { get; set; }

    // קשר למשתמש - לכל משתמש יש רשימת מקומות שמורים משלו
    public int UserId { get; set; }
    public User User { get; set; }

    // נתוני המקום (כדי שלא נצטרך לפנות לגוגל כל פעם מחדש עבור השם)
    public string PlaceName { get; set; } = string.Empty;
    public string GooglePlaceId { get; set; } = string.Empty;
    public string? Address { get; set; }

    // מתי המקום נשמר (נחמד למיון לפי תאריך)
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}