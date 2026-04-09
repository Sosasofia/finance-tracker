using AutoMapper;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Application.Features.Users.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthInfrastructureService _authInfraService;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IAuthInfrastructureService authInfraService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _authInfraService = authInfraService;
        _mapper = mapper;
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
            User = _mapper.Map<UserResponse>(user)
        };
    }
}
