using Microsoft.AspNetCore.Mvc;
using MediatR;
using TravelPlanner.Application.Features.Users.Commands.RegisterUser;
using TravelPlanner.Application.Features.Users.Commands.LoginUser; 
using TravelPlanner.Application.Features.Users.Queries.GetUserProfile;
using TravelPlanner.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TravelPlanner.Application.Features.Users.Commands.UpdateUserProfile;
using TravelPlanner.Application.Features.Users.Queries.GetUserTravelContext;
using TravelPlanner.Domain.DTOs;

namespace TravelPlanner.WebApi.Controllers;

// הגדרת המחלקה כ-Controller של API עם נתיב בסיסי api/users
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    // הזרקת ה-Mediator לניהול התקשורת עם ה-Handlers בשכבת ה-Application
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }


    /// רישום משתמש חדש - נשאר פתוח לכולם

    [HttpPost("register")] // נתיב: POST api/users/register
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        try
        {
            // שליחת הפקודה ל-RegisterUserHandler
            var userDto = await _mediator.Send(command);
            // החזרת תשובת הצלחה עם מזהה המשתמש שנוצר
            //return Ok(new { userDto, Message = "User registered successfully" });
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// התחברות משתמש וקבלת טוקן (JWT)
    /// </summary>
    [HttpPost("login")] // נתיב: POST api/users/login
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        try
        {
            // שליחת פקודת ההתחברות ל-LoginUserHandler
            // ה-Handler יבדוק סיסמה ויחזיר UserDto עם טוקן
            var userDto = await _mediator.Send(command);

            // החזרת אובייקט המשתמש המלא כולל המפתח הדיגיטלי (Token)
            return Ok(userDto);
        }

        catch (Exception ex)
        {

            return BadRequest(new { message = ex.Message });
        }

        }

    /// <summary>
    /// שליפת פרופיל משתמש לפי ID
    /// </summary>
    [Authorize] // השורה הזו מוודאת שהמשתמש שלח JWT תקין ב-Header של הבקשה
    [HttpGet("profile")] // נתיב: GET api/users/1
    public async Task<ActionResult<UserDto>> GetProfile()
    {

        // שליפת ה-ID מהטוקן בצורה מאובטחת
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("לא נמצא מזהה משתמש בטוקן");
        }

        int userId = int.Parse(userIdClaim);

        // יצירת שאילתה לשליפת נתונים
        var query = new GetUserProfileQuery(userId);
        var userProfile = await _mediator.Send(query);

        // אם המשתמש לא נמצא, החזרת שגיאת 404
        if (userProfile == null)
        {
            return NotFound(new { message = "המשתמש לא נמצא במערכת" });
        }

        return Ok(userProfile);
    }

    /// <summary>
    /// עדכון פרטי פרופיל משתמש
    /// </summary>
    [Authorize]
    [HttpPut("profile")] // נתיב: PUT api/users/profile
    public async Task<IActionResult> UpdateProfile(UpdateUserProfileCommand command)
    {
        // שליפת ה-ID מהטוקן כדי לוודא שמשתמש מעדכן רק את עצמו
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        int userIdFromToken = int.Parse(userIdClaim);

        // יצירת פקודה חדשה המבוססת על הנתונים שנשלחו, עם ה-ID המאובטח מהטוקן
        var secureCommand = command with { UserId = userIdFromToken };

        var result = await _mediator.Send(secureCommand);

        if (!result) return NotFound("המשתמש לא נמצא");

        return Ok(new { message = "הפרופיל עודכן בהצלחה" });
    }


    /// <summary>
    /// שליפת הקשר הנסיעה של המשתמש (עבור רכיב ה-AI)
    /// </summary>
    [Authorize]
    [HttpGet("travel-context")] // נתיב: GET api/users/travel-context
    public async Task<ActionResult<UserPreferencesDto>> GetTravelContext()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var query = new GetUserTravelContextQuery(userId);
        var context = await _mediator.Send(query);

        return Ok(context);
    }
}






