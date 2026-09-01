using MediatR;

namespace TravelPlanner.Application.Features.Users.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(
    int UserId,
    string FirstName,
    string LastName,
    string HomeAddress,
    string Email,   
    List<int> ChildrenAges,         
    bool PreferAccessiblePaths       
    ) : IRequest<bool>;