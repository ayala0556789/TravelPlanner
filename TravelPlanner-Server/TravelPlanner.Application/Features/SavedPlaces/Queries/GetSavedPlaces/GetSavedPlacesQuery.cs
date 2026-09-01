using MediatR;
using TravelPlanner.Application.DTOs;


namespace TravelPlanner.Application.Features.SavedPlaces.Queries.GetSavedPlaces;

// השאילתה מבקשת את כל המקומות השמורים עבור משתמש ספציפי
public record GetSavedPlacesQuery(int UserId) : IRequest<IEnumerable<SavedPlaceDto>>;