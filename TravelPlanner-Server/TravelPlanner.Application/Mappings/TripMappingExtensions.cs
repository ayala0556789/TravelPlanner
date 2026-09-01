using TravelPlanner.Application.DTOs;
using TravelPlanner.Domain.Entities;

public static class TripMappingExtensions
{
    public static TripDto ToDto(this Trip trip)
    {
        return new TripDto
        {
            Id = trip.Id,
            Destination = trip.Destination,
            TravelDate = trip.TravelDate,
            DurationInHours = trip.DurationInHours,
            EquipmentList = trip.EquipmentList,
            Places = trip.TripPlaces.Select(p => p.ToDto()).ToList()
        };
    }

    public static TripPlaceDto ToDto(this TripPlace place)
    {
        return new TripPlaceDto
        {
            Id = place.Id,
            PlaceName = place.PlaceName,
            GooglePlaceId = place.GooglePlaceId,
            Latitude = place.Latitude,
            Longitude = place.Longitude,
            OrderIndex = place.OrderIndex,
            Description = place.Description

        };
    }
}