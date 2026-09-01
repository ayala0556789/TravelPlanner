using MediatR;
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Application.Features.Trips.Commands.UpdateEquipment;

public class UpdateEquipmentHandler : IRequestHandler<UpdateEquipmentCommand>
{
    private readonly ITripRepository _tripRepository;

    public UpdateEquipmentHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
    {
        // קריאה לפונקציה שעדכנו קודם ברפוזיטורי
        await _tripRepository.UpdateEquipmentListAsync(request.TripId, request.UserId, request.EquipmentJson);

        // שמירת השינויים בבסיס הנתונים
        await _tripRepository.SaveChangesAsync();
    }
}