using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelPlanner.Application.DTOs; // הוספת ה-Namespace של ה-DTO
using TravelPlanner.Application.Features.Trips.Commands.CreateTrip;
using TravelPlanner.Application.Features.Trips.Commands.DeleteTrip;
using TravelPlanner.Application.Features.Trips.Commands.UpdateEquipment;
using TravelPlanner.Application.Features.Trips.Commands.UpdatePlacesOrder;
using TravelPlanner.Application.Features.Trips.Queries.GetEquipment;
using TravelPlanner.Application.Features.Trips.Queries.GetTripById;
using TravelPlanner.Application.Features.Trips.Queries.GetUserTrips;
using TravelPlanner.Domain.Entities;

namespace TravelPlanner.WebApi.Controllers;

[Authorize] // חסימת ה-Controller למשתמשים מחוברים בלבד
[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TripsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // עוזר פרטי כדי לא לחזור על הקוד של שליפת ה-ID בכל פונקציה
    private string GetUserIdFromToken()
    {
        // בגלל ה-Clear() ב-Program.cs, זה תמיד יהיה "nameid"
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
           ?? User.FindFirst("nameid")?.Value;
    }


    /// יצירת טיול חדש
    [HttpPost]
    // --- שייך ל-Repository: AddTripAsync ---
    public async Task<ActionResult<int>> Create([FromBody] CreateTripRequest request)
    {

        // אם הנתונים לא הגיעו, ה-ModelState יהיה לא תקין
        //if (request == null || !ModelState.IsValid)
        //{
        //    return BadRequest(ModelState);
        //}
        // 1. חילוץ ה-ID מהטוקן
        var userIdClaim = GetUserIdFromToken();
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("מזהה משתמש לא נמצא בטוקן");

        int userId = int.Parse(userIdClaim);

        // 2. בניית ה-Command עם הנתונים מה-Request וה-ID מהטוקן
        // זה מחבר בין מה שהגיע מהאנגולר לבין הזהות המאובטחת של המשתמש
        var command = new CreateTripCommand(
         request.Destination,     // string Destination
         request.TravelDate,       // DateTime StartDate
         request.DurationInHours, // int DurationInHours
         request.Category,
         userId,                  // int UserId
        request.Budget,
        request.SelectedChildrenAges
         );

        var tripId = await _mediator.Send(command);
        return Ok(tripId);
    }



    /// שליפת היסטוריית טיולים של משתמש
    [HttpGet]
    // --- שייך ל-Repository: GetUserTripsAsync ---
    public async Task<ActionResult<IEnumerable<TripDto>>> GetUserTrips()
    {

        // שליפת ה-ID מתוך ה-Claims של הטוקן (JWT)
        //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        //    ?? User.FindFirst("nameid")?.Value;
        var userIdClaim = GetUserIdFromToken();

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("לא נמצא מזהה משתמש בטוקן");
        }

        int userId = int.Parse(userIdClaim);

        // יצירת השאילתה עבור המשתמש הספציפי
        var query = new GetUserTripsQuery(userId);

        // שליחת השאילתה ל-MediatR. בזכות השינוי ב-Handler, 
        // המשתנה trips יכיל עכשיו רשימה נקייה ללא שדות null
        var trips = await _mediator.Send(query);

        // החזרת הרשימה הנקייה ל-Angular
        return Ok(trips);
    }

    [HttpDelete("{id}")]
    // --- שייך ל-Repository: DeleteTripAsync ---
    public async Task<IActionResult> Delete(int id)
    {
        // חילוץ ה-ID של המשתמש מהטוקן בצורה מאובטחת
        var userIdClaim = GetUserIdFromToken(); //User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        int userId = int.Parse(userIdClaim);

        // שליחת הפקודה ל-MediatR
        await _mediator.Send(new DeleteTripCommand(id, userId));

        return NoContent(); // מחזירים 204 כי המחיקה הצליחה ואין תוכן להחזיר
    }

    // שליפת רשימת ציוד לטיול ספציפי
    [HttpGet("{id}/equipment")]
    // --- שייך ל-Repository: GetEquipmentListAsync ---
    public async Task<ActionResult<string>> GetEquipment(int id)
    {
        var userIdClaim = GetUserIdFromToken();//User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var query = new GetEquipmentQuery(id, userId);
        var equipment = await _mediator.Send(query);

        return Ok(new { equipment }); // מחזירים כאובייקט כדי שיהיה קל לקרוא באנגולר
    }

    // עדכון רשימת הציוד
    [HttpPut("{id}/equipment")]
    // --- שייך ל-Repository: UpdateEquipmentListAsync ---
    public async Task<IActionResult> UpdateEquipment(int id, [FromBody] string equipmentJson)
    {
        var userIdClaim = GetUserIdFromToken();//User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var command = new UpdateEquipmentCommand(id, userId, equipmentJson);
        await _mediator.Send(command);

        return NoContent();
    }

    // עדכון סדר המקומות בטיול (Drag & Drop)
    [HttpPut("{id}/order")]
    // --- שייך ל-Repository: UpdateTripPlacesOrderAsync ---
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] List<TripPlace> updatedPlaces)
    {
        // בדיקת אימות בסיסית (לוודא שהמשתמש מחובר)
        var userIdClaim = GetUserIdFromToken();//User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        // שליחת הפקודה לעדכון הסדר
        var command = new UpdatePlacesOrderCommand(id, updatedPlaces);
        await _mediator.Send(command);

        return NoContent();
    }

    // --- !!! חדש !!! שייך ל-Repository: GetTripWithPlacesAsync ---
    [HttpGet("{id}")]
    public async Task<ActionResult<TripDto>> GetById(int id)
    {
        // חילוץ מזהה המשתמש מהטוקן
        var userIdClaim = GetUserIdFromToken(); //User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        int userId = int.Parse(userIdClaim);
        // יצירת השאילתה עם ה-ID של הטיול וה-ID של המשתמש שחולץ מהטוקן
        var query = new GetTripByIdQuery(id, userId);
        var trip = await _mediator.Send(query);

        if (trip == null) return NotFound("הטיול לא נמצא");
        return Ok(trip);
    }

}