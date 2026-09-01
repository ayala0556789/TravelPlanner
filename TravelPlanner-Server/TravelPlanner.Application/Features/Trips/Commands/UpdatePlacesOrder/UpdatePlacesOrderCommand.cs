using MediatR;
using TravelPlanner.Domain.Entities;

namespace TravelPlanner.Application.Features.Trips.Commands.UpdatePlacesOrder;

// הפקודה מקבלת את מזהה הטיול ואת רשימת המקומות המעודכנת עם האינדקסים החדשים
public record UpdatePlacesOrderCommand(int TripId, List<TripPlace> UpdatedPlaces) : IRequest;