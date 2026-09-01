using MediatR;

namespace TravelPlanner.Application.Features.Trips.Commands.UpdateEquipment;

// צריך את ה-ID של הטיול, ה-ID של המשתמש (לאבטחה) ואת המחרוזת של הציוד
public record UpdateEquipmentCommand(int TripId, int UserId, string EquipmentJson) : IRequest;