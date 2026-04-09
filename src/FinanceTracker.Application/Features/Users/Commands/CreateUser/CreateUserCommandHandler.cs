using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var existingUser = await _userRepository.FindByEmailAsync(command.Email);
        if (existingUser != null) throw new Exception("User already exists.");

        var user = User.Create(
            command.Email,
            command.Name,
            command.Password,
            command.Provider
        );
        await _userRepository.AddAsync(user);

        return user.Id;
    }
}
