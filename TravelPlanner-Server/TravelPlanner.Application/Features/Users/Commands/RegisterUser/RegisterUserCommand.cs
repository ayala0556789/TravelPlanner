using MediatR;
using TravelPlanner.Application.DTOs;

namespace TravelPlanner.Application.Features.Users.Commands.RegisterUser;

public record RegisterUserCommand(
    string IdentityNumber,
    string FirstName,
    string LastName,
    string Password,
    string HomeAddress,
    string Email,
     List<int> ChildrenAges,          // : רשימת גילאי הילדים
    bool PreferAccessiblePaths       // : נגישות קבועה
) : IRequest<UserDto>;