using MediatR;
using TravelPlanner.Application.DTOs; 
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Application.Features.SavedPlaces.Queries.GetSavedPlaces;

public class GetSavedPlacesHandler : IRequestHandler<GetSavedPlacesQuery, IEnumerable<SavedPlaceDto>>
{
    private readonly ISavedPlaceRepository _repository;

    public GetSavedPlacesHandler(ISavedPlaceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SavedPlaceDto>> Handle(GetSavedPlacesQuery request, CancellationToken cancellationToken)
    {
        var places = await _repository.GetSavedPlacesByUserIdAsync(request.UserId);

        // המרה (Mapping) מהיישות של ה-Domain ל-DTO של ה-Application
        return places.Select(p => new SavedPlaceDto
        {
            Id = p.Id,
            PlaceName = p.PlaceName,
            GooglePlaceId = p.GooglePlaceId,
            Address = p.Address,
            SavedAt = p.SavedAt
        });
    }
}