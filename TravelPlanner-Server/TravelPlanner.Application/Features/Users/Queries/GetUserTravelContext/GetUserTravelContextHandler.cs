using MediatR;
using TravelPlanner.Domain.DTOs;
using TravelPlanner.Domain.Interfaces;

namespace TravelPlanner.Application.Features.Users.Queries.GetUserTravelContext;

public class GetUserTravelContextHandler : IRequestHandler<GetUserTravelContextQuery, UserPreferencesDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserTravelContextHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserPreferencesDto> Handle(GetUserTravelContextQuery request, CancellationToken cancellationToken)
    {
        // שימוש בפונקציה הקיימת ברפוזיטורי 
        return await _userRepository.GetUserTravelContextAsync(request.UserId);
    }
}