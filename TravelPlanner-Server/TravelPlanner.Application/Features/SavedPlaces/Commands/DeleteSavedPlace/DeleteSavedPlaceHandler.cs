using MediatR;
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Application.Features.SavedPlaces.Commands.DeleteSavedPlace;

public class DeleteSavedPlaceHandler : IRequestHandler<DeleteSavedPlaceCommand>
{
    private readonly ISavedPlaceRepository _repository;

    public DeleteSavedPlaceHandler(ISavedPlaceRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteSavedPlaceCommand request, CancellationToken cancellationToken)
    {

        // ביצוע המחיקה דרך ה-Repository
        await _repository.DeleteSavedPlaceAsync(request.PlaceId,request.UserId);

        // שמירת השינויים ב-DB
        await _repository.SaveChangesAsync();
    }
}