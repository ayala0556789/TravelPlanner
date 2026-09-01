using MediatR;

namespace TravelPlanner.Application.Features.Trips.Commands.DeleteTrip;

// ה-Command מגדיר מה אנחנו צריכים כדי לבצע מחיקה: מזהה טיול ומזהה משתמש (לאבטחה)
public record DeleteTripCommand(int TripId, int UserId) : IRequest;