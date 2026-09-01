using MediatR;
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Application.Features.Trips.Commands.DeleteTrip;

public class DeleteTripHandler : IRequestHandler<DeleteTripCommand>
{
    private readonly ITripRepository _tripRepository;

    public DeleteTripHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task Handle(DeleteTripCommand request, CancellationToken cancellationToken)
    {
        // 1. קריאה לפונקציה שכתבת ברפוזיטורי
        await _tripRepository.DeleteTripAsync(request.TripId, request.UserId);

        // 2. ביצוע ה-Save לשמירה סופית ב-DB
        await _tripRepository.SaveChangesAsync();
    }
}