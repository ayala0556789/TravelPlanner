using MediatR;

namespace TravelPlanner.Application.Features.Trips.Commands.CreateTrip;
// ה-Command מגדיר מה אנחנו מצפים לקבל מהמשתמש
// הוא מחזיר 'int' - המזהה (ID) של הטיול החדש שייווצר
public record CreateTripCommand(

    string Destination,
    DateTime TravelDate, 
    int DurationInHours,
    string Category,
    int UserId,
    string Budget, 
    List<int> SelectedChildrenAges
    ) : IRequest<int>;

