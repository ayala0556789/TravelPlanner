using MediatR;
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Application.Features.Trips.Queries.GetEquipment;

public class GetEquipmentHandler : IRequestHandler<GetEquipmentQuery, string>
{
    private readonly ITripRepository _tripRepository;

    public GetEquipmentHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<string> Handle(GetEquipmentQuery request, CancellationToken cancellationToken)
    {
        // שליפת הציוד בצורה מאובטחת מהרפוזיטורי
        return await _tripRepository.GetEquipmentListAsync(request.TripId, request.UserId);
    }
}