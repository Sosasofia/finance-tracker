using FinanceTracker.Application.Features.Users.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var existingUser = await _userRepository.FindByEmailAsync(command.Email);
        if (existingUser != null)
        {
            throw new DuplicateException($"User with email '{command.Email}' already exists.");
        }

        var user = User.Create(
            command.Email,
            command.Name ?? string.Empty,
            command.Password,
            command.Provider
        );

        await _userRepository.AddAsync(user);

        return UserResponse.MapFrom(user);
    }
}
