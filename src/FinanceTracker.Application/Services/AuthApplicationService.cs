using AutoMapper;

using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Application.Features.Users;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public class AuthApplicationService : IAuthApplicationService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthInfrastructureService _authInfraService;
    private readonly IMapper _mapper;

    public AuthApplicationService(
        IUserRepository userRepository,
        IAuthInfrastructureService authInfraService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _authInfraService = authInfraService;
        _mapper = mapper;
    }


    public async Task<AuthResponse> RegisterUserAsync(string email, string password)
    {
        if (await _userRepository.ExistsByEmailAsync(email) != null)
        {
            throw new Exception("Email already registered");
        }

        var passwordHash = _authInfraService.HashPassword(password);

        var user = new User
        {
            Email = email,
            Password = passwordHash,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        user.Password = passwordHash; 


        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var token = _authInfraService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<AuthResponse> LoginUserAsync(string email, string password)
    {
        var user = await _userRepository.ExistsByEmailAsync(email);

        if (user == null || !_authInfraService.VerifyPassword(password, user.Password))
        {
            throw new Exception("Invalid credentials");
        }

        var token = _authInfraService.GenerateToken(user);

        user.LastLoginAt = DateTime.UtcNow; 
        await _userRepository.SaveChangesAsync();

        return new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserDto>(user)
        };
    }
}
