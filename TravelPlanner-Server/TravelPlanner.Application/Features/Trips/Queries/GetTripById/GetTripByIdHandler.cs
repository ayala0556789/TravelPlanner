using MediatR;
using TravelPlanner.Application.DTOs;
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Application.Features.Trips.Queries.GetTripById;

public class GetTripByIdHandler : IRequestHandler<GetTripByIdQuery, TripDto?>
{
    private readonly ITripRepository _tripRepository;

    public GetTripByIdHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<TripDto?> Handle(GetTripByIdQuery request, CancellationToken cancellationToken)
    {
        // שימוש בפונקציה מהרפוזיטורי שכוללת את ה-Include למקומות
        var trip = await _tripRepository.GetTripWithPlacesAsync(request.TripId, request.UserId);

        return trip?.ToDto();
        //if (trip == null) return null;

        //// מיפוי ל-DTO כולל המקומות
        //return new TripDto
        //{
        //    Id = trip.Id,
        //    Destination = trip.Destination,
        //    TravelDate = trip.TravelDate,
        //    DurationInHours = trip.DurationInHours,
        //    EquipmentList = trip.EquipmentList,
        //    // כאן אנחנו ממפים גם את רשימת המקומות!
        //    Places = trip.TripPlaces.Select(p => new TripPlaceDto
        //    {
        //        Id = p.Id,
        //        PlaceName = p.PlaceName,
        //        Latitude = p.Latitude,
        //        Longitude = p.Longitude,
        //        OrderIndex = p.OrderIndex
        //    }).ToList()
        //};
    }
}