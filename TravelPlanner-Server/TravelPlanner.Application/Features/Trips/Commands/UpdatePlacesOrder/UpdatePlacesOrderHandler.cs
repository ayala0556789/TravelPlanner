using MediatR;
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Application.Features.Trips.Commands.UpdatePlacesOrder;

public class UpdatePlacesOrderHandler : IRequestHandler<UpdatePlacesOrderCommand>
{
    private readonly ITripRepository _tripRepository;

    public UpdatePlacesOrderHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task Handle(UpdatePlacesOrderCommand request, CancellationToken cancellationToken)
    {
        // קריאה לפונקציה ברפוזיטורי שמעדכנת רק את ה-OrderIndex
        await _tripRepository.UpdateTripPlacesOrderAsync(request.TripId, request.UpdatedPlaces);

        // שמירה סופית של הסדר החדש
        await _tripRepository.SaveChangesAsync();
    }
}