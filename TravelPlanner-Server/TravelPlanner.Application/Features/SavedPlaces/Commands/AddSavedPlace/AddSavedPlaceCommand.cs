using MediatR;

namespace TravelPlanner.Application.Features.SavedPlaces.Commands.AddSavedPlace;

// הפקודה מגדירה אילו נתונים אנחנו צריכים לקבל כדי לשמור מקום
public record AddSavedPlaceCommand(
    int UserId,
    string PlaceName,
    string GooglePlaceId,
    string? Address
) : IRequest<int>; // הפקודה תחזיר את ה-ID של המקום החדש שנוצר