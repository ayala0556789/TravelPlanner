namespace TravelPlanner.Domain.Entities;

// השתמשנו ב-public כדי שפרויקטים אחרים ב-Solution יוכלו לגשת למחלקה הזו
public class User
{
    // Id הוא המפתח הראשי (Primary Key). EF Core מזהה אוטומטית שזה ה-ID של הטבלה
    public int Id { get; set; }

    // תעודת זהות של המשתמש - נשתמש בה לצורך התחברות (Login)
    public string IdentityNumber { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    // אנחנו לא שומרים סיסמה כטקסט פשוט מטעמי אבטחה, אלא "Hash" (ייצוג מוצפן)
    public string PasswordHash { get; set; }
    
    public string Email { get; set; }
    
    // כתובת המגורים כדי שהמערכת תדע מאיפה להתחיל את מסלול הטיול
    public string HomeAddress { get; set; }

    // שדות בוליאניים (כן/לא) שיעזרו ל-AI לסנן מקומות רלוונטיים (נגישות לעגלה וכו')
 

    public string? ChildrenAges { get; set; } // נשמור כ-JSON או מחרוזת: "1,3,5"
    public bool PreferAccessiblePaths { get; set; } // נגישות כברירת מחדל

    // הגדרה של קשר "אחד לרבים": משתמש אחד (1) יכול שיהיו לו הרבה (Many) טיולים
    // אנחנו מאתחלים רשימה ריקה כדי למנוע שגיאות של Null בהמשך
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}