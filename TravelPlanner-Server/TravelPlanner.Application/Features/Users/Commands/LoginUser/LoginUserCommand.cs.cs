using MediatR;
using TravelPlanner.Application.DTOs;

namespace TravelPlanner.Application.Features.Users.Commands.LoginUser;

// המשתמש שולח תעודת זהות וסיסמה כדי להתחבר
public record LoginUserCommand(string IdentityNumber, string Password) : IRequest<UserDto>;