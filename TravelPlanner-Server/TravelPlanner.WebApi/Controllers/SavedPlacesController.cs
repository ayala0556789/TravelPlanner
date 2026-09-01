using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using TravelPlanner.Application.Features.SavedPlaces.Commands.AddSavedPlace;
using TravelPlanner.Application.Features.SavedPlaces.Commands.DeleteSavedPlace;
using TravelPlanner.Application.Features.SavedPlaces.Queries.GetSavedPlaces;
using TravelPlanner.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace TravelPlanner.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // אבטחה: רק משתמש עם טוקן JWT בתוקף יכול לגשת לנקודות הקצה כאן

public class SavedPlacesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SavedPlacesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// שמירת מקום חדש במועדפים
    /// </summary>
    //[HttpPost]
  
    [HttpPost]
    public async Task<ActionResult<int>> AddSavedPlace(AddSavedPlaceRequest request)
    {
        // 1. שליפת ה-ID מהטוקן בצורה מאובטחת
        // המערכת מוודאת שזה המשתמש המחובר באמת
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);


        // 2. יצירת ה-Command (הפקודה)
        // כאן אנחנו "מחברים" את הנתונים מהאנגולר (request) עם ה-ID המאובטח (userId)
        var command = new AddSavedPlaceCommand(
            userId,
            request.PlaceName,
            request.GooglePlaceId,
            request.Address
        );

        // 3. שליחה ל-Mediator
        // ה-Mediator יפעיל את ה-Handler, שישתמש ב-SavedPlaceRepository שהעלית
        var id = await _mediator.Send(command);

        return Ok(id);
    }

    /// <summary>
    /// שליפת כל המקומות השמורים של משתמש מסוים
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SavedPlaceDto>>> GetUserSavedPlaces()
    {

        // חילוץ ה-ID מהטוקן בצורה בטוחה
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("לא נמצא מזהה משתמש בטוקן");
        }
        int userId = int.Parse(userIdClaim);
        // שליחת השאילתה ל-Handler לקבלת רשימת המקומות
        var query = new GetSavedPlacesQuery(userId);
        var places = await _mediator.Send(query);
        return Ok(places);
    }

    /// <summary>
    /// מחיקת מקום מהמועדפים
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSavedPlace(int id)
    {
        // שליפת ה-ID של המשתמש מהטוקן המחובר
        var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        // שליחת פקודת מחיקה לפי ה-ID של המקום השמור
        // יצירת ה-Record באמצעות הבנאי שלו
        var command = new DeleteSavedPlaceCommand(id, UserId);
        await _mediator.Send(command);
        return NoContent(); // מחזיר תשובה של "אין תוכן" - הצלחה ללא נתונים חוזרים
    }
}