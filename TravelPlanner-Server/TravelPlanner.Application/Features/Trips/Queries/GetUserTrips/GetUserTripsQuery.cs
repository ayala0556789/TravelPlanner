using MediatR;
using TravelPlanner.Application.DTOs;


namespace TravelPlanner.Application.Features.Trips.Queries.GetUserTrips;

// השאילתה מקבלת מזהה משתמש ומחזירה רשימה של טיולים
public record GetUserTripsQuery(int UserId) : IRequest<IEnumerable<TripDto>>;