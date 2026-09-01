using MediatR;
using TravelPlanner.Application.DTOs;

namespace TravelPlanner.Application.Features.Users.Queries.GetUserProfile;

// השאילתה מקבלת מזהה משתמש ומחזירה אובייקט DTO (ללא סיסמה)
public record GetUserProfileQuery(int UserId) : IRequest<UserDto>;