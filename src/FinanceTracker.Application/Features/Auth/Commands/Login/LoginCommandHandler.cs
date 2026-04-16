using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
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

        var userSession = new UserSessionDto(
            user.Id,
            user.Email,
            user.Name ?? "User",
            user.Role ?? "User"
        );

        return new AuthResponseDto(token, userSession);
    }
}
