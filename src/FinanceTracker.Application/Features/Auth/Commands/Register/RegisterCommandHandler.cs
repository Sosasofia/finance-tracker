using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthInfrastructureService _authInfrastructureService;

    public RegisterCommandHandler(IUserRepository userRepository, IAuthInfrastructureService authInfrastructureService)
    {
        _userRepository = userRepository;
        _authInfrastructureService = authInfrastructureService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand command, CancellationToken ct)
    {
        var existing = await _userRepository.FindByEmailAsync(command.Email);
        if (existing != null) throw new DuplicateException("User already registered.");

        var hashedPassword = _authInfrastructureService.HashPassword(command.Password);

        var user = User.Create(
            command.Email,
            command.Name,
            hashedPassword,
            "local"
        );

        await _userRepository.AddAsync(user);

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
