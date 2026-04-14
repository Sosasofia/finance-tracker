using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.Auth.Commands.GoogleLogin;

public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthInfrastructureService _authInfrastructureService;

    public GoogleLoginCommandHandler(
        IUserRepository userRepository,
        IAuthInfrastructureService authInfrastructureService)
    {
        _userRepository = userRepository;
        _authInfrastructureService = authInfrastructureService;
    }

    public async Task<AuthResponseDto> Handle(GoogleLoginCommand command, CancellationToken ct)
    {
        var payload = await _authInfrastructureService.ValidateGoogleToken(command.IdToken)
            ?? throw new UnauthorizedAccessException("Invalid Google Token");

        var user = await _userRepository.FindByEmailAsync(payload.Email);

        if (user == null)
        {
            user = User.Create(
                payload.Email,
                payload.Name,
                null!,
                "google"
            );

            user.UpdateProfile(payload.Name, payload.Picture);

            await _userRepository.AddAsync(user);
        }
        else
        {
            user.RecordLogin();
            await _userRepository.UpdateAsync(user);
        }

        var token = _authInfrastructureService.GenerateToken(
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
