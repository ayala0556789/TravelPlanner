using BCrypt.Net;
using MediatR;
using TravelPlanner.Application.DTOs;
using TravelPlanner.Domain.Interfaces;
using TravelPlanner.Application.Common.Interfaces; // הממשק שיצרנו

namespace TravelPlanner.Application.Features.Users.Commands.LoginUser;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator; // הזרקת השירות

    public LoginUserHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<UserDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // 1. חיפוש המשתמש לפי תעודת זהות
        var user = await _userRepository.GetByIdentityNumberAsync(request.IdentityNumber);

        // 2. אימות הסיסמה המוצפנת (השוואה של הטקסט הנקי מול ה-Hash ב-DB)
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new Exception("תעודת זהות או סיסמה אינם נכונים");
        }

        // 3. יצירת טוקן JWT אמיתי עבור המשתמש
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.FirstName);

        // 4. החזרת הנתונים לאנגולר כולל הטוקן
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IdentityNumber = user.IdentityNumber,
            HomeAddress = user.HomeAddress, 
            Email = user.Email,
            Token = token // המפתח הדיגיטלי שישמש את המשתמש להמשך הגלישה
        };
    }
}


//using BCrypt.Net;
//using MediatR;
//using TravelPlanner.Application.DTOs;
//using TravelPlanner.Domain.Interfaces;

//namespace TravelPlanner.Application.Features.Users.Commands.LoginUser;

//public class LoginUserHandler : IRequestHandler<LoginUserCommand, UserDto>
//{
//    private readonly IUserRepository _userRepository;

//    public LoginUserHandler(IUserRepository userRepository)
//    {
//        _userRepository = userRepository;
//    }

//    public async Task<UserDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
//    {
//        // 1. חיפוש המשתמש בבסיס הנתונים לפי תעודת זהות
//        var user = await _userRepository.GetByIdentityNumberAsync(request.IdentityNumber);

//        // 2. בדיקה האם המשתמש קיים והאם הסיסמה נכונה (Verify משווה את הטקסט ל-Hash)
//        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
//        {
//            throw new Exception("תעודת זהות או סיסמה אינם נכונים");
//        }

//        // 3. החזרת ה-DTO של המשתמש (כאן בעתיד נוסיף את הטוקן)
//        return new UserDto
//        {
//            Id = user.Id,
//            FirstName = user.FirstName,
//            LastName = user.LastName,
//            IdentityNumber = user.IdentityNumber
//        };
//    }
//}