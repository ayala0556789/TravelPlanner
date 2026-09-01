using MediatR;
using TravelPlanner.Application.DTOs;
using TravelPlanner.Domain.Interfaces;
using System.Text.Json;

namespace TravelPlanner.Application.Features.Users.Queries.GetUserProfile;

// המחלקה מיישמת את הממשק IRequestHandler
// היא מקבלת שאילתה מסוג GetUserProfileQuery ומחזירה אובייקט מסוג UserDto
public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    // הזרקת התלות של ה-Repository כדי שנוכל לגשת לבסיס הנתונים
    public GetUserProfileHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // הפונקציה המרכזית שמבצעת את הלוגיקה של השאילתה
    public async Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        // שליפת המשתמש מבסיס הנתונים לפי ה-ID שהגיע מהשאילתה
        var user = await _userRepository.GetByIdAsync(request.UserId);

        // בדיקת בטיחות: אם המשתמש לא נמצא, נחזיר null (ריק)
        if (user == null) return null;

        // מיפוי (Mapping): העברת הנתונים מהיישות של בסיס הנתונים (Entity) ל-DTO.
        // אנחנו מחזירים רק את השדות שחשוב לנו להציג במסך האנגולר,
        // ובכך נמנעים משליחת מידע רגיש כמו ה-PasswordHash (הסיסמה המוצפנת).
        return new UserDto
        {
            Id = user.Id,                 // המזהה הייחודי
            IdentityNumber = user.IdentityNumber, // מספר הזהות
            FirstName = user.FirstName,   // שם פרטי
            LastName = user.LastName,     // שם משפחה
            HomeAddress = user.HomeAddress, // כתובת המגורים
            Email = user.Email,
            ChildrenAges = string.IsNullOrEmpty(user.ChildrenAges)
            ? new List<int>()
            : JsonSerializer.Deserialize<List<int>>(user.ChildrenAges) ?? new List<int>(),  // חדש
            PreferAccessiblePaths = user.PreferAccessiblePaths  // חדש
        };
    }
}