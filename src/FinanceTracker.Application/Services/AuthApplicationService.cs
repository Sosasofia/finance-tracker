using AutoMapper;
using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Application.Features.Users;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Services;

public class AuthApplicationService : IAuthApplicationService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IAuthInfrastructureService _authInfraService;
    private readonly IMapper _mapper;

    public AuthApplicationService(
        IUserRepository userRepository,
        IUserService userService,
        IAuthInfrastructureService authInfraService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _userService = userService;
        _authInfraService = authInfraService;
        _mapper = mapper;
    }


    public async Task<AuthResponseDto> RegisterUserAsync(string email, string password)
    {
        if (await _userRepository.ExistsByEmailAsync(email))
        {
            throw new DuplicateException("Email already registered");
        }

        var passwordHash = _authInfraService.HashPassword(password);

        var newUser = new PasswordAuthDto { Email = email, Password = passwordHash };

        var savedUser = await _userService.CreateUser(newUser);
        var token = _authInfraService.GenerateToken(savedUser);

        return new AuthResponseDto
        {
            Token = token,
            User = _mapper.Map<UserResponse>(savedUser)
        };
    }

    public async Task<AuthResponseDto> LoginUserAsync(string email, string password)
    {
        var user = await _userRepository.FindByEmailAsync(email);

        bool passwordCorrect = false;

        if(user != null)
        {
            passwordCorrect = _authInfraService.VerifyPassword(password, user.Password);
        }

        if (user == null || !passwordCorrect)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var mappedUser = _mapper.Map<UserDto>(user);
        var token = _authInfraService.GenerateToken(mappedUser);

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        return new AuthResponseDto
        {
            Token = token,
            User = _mapper.Map<UserResponse>(mappedUser)
        };
    }

    public async Task<AuthResponseDto> AuthenticateWithGoogleAsync(string idToken)
    {
        GoogleTokenPayload payload;

        try
        {
            payload = await _authInfraService.ValidateGoogleToken(idToken);
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException("Invalid Google id token.", ex);
        }

        var user = await _userService.GetByEmail(payload.Email);

        var googleAuth = new GoogleAuthDto { Email = payload.Email, Name = payload.Name };


        if (user == null)
        {
            user = await _userService.CreateUser(googleAuth);
        }
        else
        {
            user = await _userService.UpdateUser(user, googleAuth);
        }

        var token = _authInfraService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            User = _mapper.Map<UserResponse>(user)
        };
    }
}
