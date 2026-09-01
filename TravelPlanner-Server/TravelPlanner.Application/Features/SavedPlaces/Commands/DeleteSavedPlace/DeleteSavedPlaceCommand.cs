using MediatR;

namespace TravelPlanner.Application.Features.SavedPlaces.Commands.DeleteSavedPlace;

// למחיקה אנחנו צריכים רק את המזהה של המקום השמור
public record DeleteSavedPlaceCommand(int PlaceId , int UserId) : IRequest;