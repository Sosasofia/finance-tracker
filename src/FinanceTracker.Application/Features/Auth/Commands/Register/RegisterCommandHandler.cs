using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthInfrastructureService _authInfrastructureService;

    public RegisterCommandHandler(IUserRepository userRepository, IAuthInfrastructureService authInfrastructureService)
    {
        _userRepository = userRepository;
        _authInfrastructureService = authInfrastructureService;
    }

    public async Task<Guid> Handle(RegisterCommand command, CancellationToken ct)
    {
        var existing = await _userRepository.FindByEmailAsync(command.Email);
        if (existing != null) throw new Exception("User already registered.");

        var hashPassword = _authInfrastructureService.HashPassword(command.Password);

        var user = User.Create(
            command.Email,
            command.Name,
            hashPassword,
            command.Provider
        );

        await _userRepository.AddAsync(user);

        return user.Id;
    }
}
