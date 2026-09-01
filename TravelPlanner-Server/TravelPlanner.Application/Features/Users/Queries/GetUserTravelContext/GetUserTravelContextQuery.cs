using MediatR;
using TravelPlanner.Domain.DTOs;

namespace TravelPlanner.Application.Features.Users.Queries.GetUserTravelContext;

// השאילתה מקבלת ID ומחזירה DTO מצומצם של העדפות
public record GetUserTravelContextQuery(int UserId) : IRequest<UserPreferencesDto>;