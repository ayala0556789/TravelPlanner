using BCrypt.Net;
using MediatR;
using TravelPlanner.Application.Common.Interfaces;
using TravelPlanner.Application.DTOs;
using TravelPlanner.Application.Features.Users.Commands.RegisterUser;
using TravelPlanner.Domain.Entities;
using TravelPlanner.Domain.Interfaces;
using System.Text.Json;

namespace TravelPlanner.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator; // הזרקת שירות הטוקנים
    public RegisterUserHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator; 
    }

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. בדיקה האם המשתמש כבר קיים (לפי אימייל או תעודת זהות)
        var existingUser = await _userRepository.GetByIdentityNumberAsync(request.IdentityNumber);
        if (existingUser != null)
        {
            throw new Exception("משתמש עם תעודת זהות זו כבר קיים במערכת");
       

        }

        // 2. הצפנת סיסמה
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 3. יצירת המשתמש
        var newUser = new User
        {
            IdentityNumber = request.IdentityNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = passwordHash, // שומרים את הסיסמה המוצפנת
            HomeAddress = request.HomeAddress,
            Email = request.Email,
            ChildrenAges = JsonSerializer.Serialize(request.ChildrenAges),        
            PreferAccessiblePaths = request.PreferAccessiblePaths
        };

        //4. שמירה בבסיס הנתונים
        await _userRepository.AddAsync(newUser);

        // 5. יצירת טוקן עבור המשתמש החדש כדי שיוכל להתחבר מיד
        var token = _jwtTokenGenerator.GenerateToken(newUser.Id, newUser.FirstName);
        

       

        //return newUser.Id;
        //6. החזרת DTO מלא כולל הטוקן
        return new UserDto
        {
            Id = newUser.Id,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            IdentityNumber = newUser.IdentityNumber,
            HomeAddress = newUser.HomeAddress,
            Email = newUser.Email,          
            ChildrenAges = request.ChildrenAges,              // חדש - כבר יש לנו את הרשימה המקורית, לא צריך לפרסר
            PreferAccessiblePaths = newUser.PreferAccessiblePaths,
            Token = token,
        };
    }
}