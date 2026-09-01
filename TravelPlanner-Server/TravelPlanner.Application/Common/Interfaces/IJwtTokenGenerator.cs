namespace TravelPlanner.Application.Common.Interfaces;

/* הגדרת הממשק ליצירת טוקן - זהו "המתכון" שה-Infrastructure יצטרך לבצע */
public interface IJwtTokenGenerator
{
    /* פונקציה שמקבלת פרטי משתמש ומחזירה מחרוזת טוקן מוצפנת */
    string GenerateToken(int userId, string firstName);
}