namespace TravelPlanner.Domain.Entities;

public class TripPlace
{
    public int Id { get; set; }

    // הקשר לטיול - כל מקום שייך לטיול ספציפי
    // הקשר לטיול: שדה זה הוא "מפתח זר" (Foreign Key) שמחבר את הנקודה לטיול ספציפי
    public int TripId { get; set; }
    // Navigation Property: מאפשר לנו לגשת לאובייקט הטיול המלא מתוך הנקודה בקוד
    public Trip Trip { get; set; }

    // נתוני המקום מ-Google Maps
    public string PlaceName { get; set; } = string.Empty;
    // מזהה ייחודי של גוגל: חשוב מאוד כדי שנוכל לשלוף מידע עדכני, תמונות ודירוגים מ-Google Maps API
    public string GooglePlaceId { get; set; } = string.Empty; // מזהה ייחודי של גוגל
    //קואורדינטות: קו רוחב וקו אורך. אלו הנתונים שבהם אנגולר תשתמש כדי להציב את ה"נעץ" על המפה
    public double Latitude { get; set; }  // קו רוחב למפה
    public double Longitude { get; set; } // קו אורך למפה

    // שדה קריטי ל-Drag & Drop
    // הוא קובע אם המקום הוא הראשון בבוקר, השני וכו'. כשנחליף סדר באנגולר, נעדכן את המספר הזה.
    public int OrderIndex { get; set; }

    // מידע נוסף (למשל מה שה-AI יכתוב על המקום)
    public string Description { get; set; } = string.Empty;
}