using MediatR;
using TravelPlanner.Domain.Interfaces;
using System.Text.Json;

namespace TravelPlanner.Application.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserProfileHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null) return false;

        // עדכון השדות
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.HomeAddress = request.HomeAddress;
        user.Email = request.Email;
        user.ChildrenAges = JsonSerializer.Serialize(request.ChildrenAges);   
        user.PreferAccessiblePaths = request.PreferAccessiblePaths;

        await _userRepository.UpdateAsync(user);
        return true;
    }
}