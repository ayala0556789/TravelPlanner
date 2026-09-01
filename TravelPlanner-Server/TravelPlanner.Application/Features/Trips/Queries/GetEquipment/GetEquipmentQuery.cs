using MediatR;

namespace TravelPlanner.Application.Features.Trips.Queries.GetEquipment;

public record GetEquipmentQuery(int TripId, int UserId) : IRequest<string>;