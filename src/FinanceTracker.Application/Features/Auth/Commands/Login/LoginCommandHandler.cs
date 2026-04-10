using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthInfrastructureService _authInfraService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IAuthInfrastructureService authInfraService)
    {
        _userRepository = userRepository;
        _authInfraService = authInfraService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand command, CancellationToken ct)
    {
        var user = await _userRepository.FindByEmailAsync(command.Email);

        if (user == null || !_authInfraService.VerifyPassword(command.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        user.RecordLogin();
        await _userRepository.UpdateAsync(user);

        var token = _authInfraService.GenerateToken(
            user.Id,
            user.Email,
            user.Name ?? string.Empty,
            user.Role ?? "User",
            user.Provider);

        return new AuthResponseDto
        {
            Token = token,
            User = new UserSessionDto(
                user.Name ?? "User",
                user.Email,
                user.ProfilePictureUrl 
            )
        };
    }
}
