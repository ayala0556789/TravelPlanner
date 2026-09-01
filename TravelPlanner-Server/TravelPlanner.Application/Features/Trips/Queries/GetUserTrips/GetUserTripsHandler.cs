using MediatR;
using TravelPlanner.Application.DTOs; // חשוב להוסיף כדי להכיר את TripDto
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Application.Features.Trips.Queries.GetUserTrips;

// שים לב ששינינו את ה-Return Type ל-IEnumerable<TripDto>
public class GetUserTripsHandler : IRequestHandler<GetUserTripsQuery, IEnumerable<TripDto>>
{
    private readonly ITripRepository _tripRepository;

    public GetUserTripsHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<IEnumerable<TripDto>> Handle(GetUserTripsQuery request, CancellationToken cancellationToken)
    {
        // שליפת הנתונים הגולמיים מה-Repository (ה-Entities)
        var trips = await _tripRepository.GetUserTripsAsync(request.UserId);

        // כאן מתבצע ה"קסם": אנחנו הופכים כל Trip ל-TripDto נקי
        // בצורה זו, שדות כמו "User" או "TripPlaces" שהיו null פשוט נעלמים מהתוצאה
        return trips.Select(t => new TripDto
        {
            Id = t.Id,
            Destination = t.Destination,
            TravelDate = t.TravelDate,
            DurationInHours = t.DurationInHours,
            EquipmentList = t.EquipmentList
        });
    }
}