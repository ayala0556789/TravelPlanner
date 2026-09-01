using MediatR;
using TravelPlanner.Application.DTOs;

namespace TravelPlanner.Application.Features.Trips.Queries.GetTripById;

// השאילתה מבקשת טיול ספציפי לפי ID, ומוודאת שהוא שייך למשתמש הנכון
public record GetTripByIdQuery(int TripId, int UserId) : IRequest<TripDto>;