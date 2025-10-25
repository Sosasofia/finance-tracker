using AutoMapper;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Application.Features.Users;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<bool> ExistsByAsync(Guid id)
    {
        try
        {
            return await _userRepository.ExistsByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception();
        }
    }

    public async Task<UserDto> GetByEmail(string email)
    {
        var user = await _userRepository.FindByEmailAsync(email);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateUser(BaseAuthDto authDto)
    {
        var newUser = _mapper.Map<User>(authDto);
        newUser.CreatedAt = DateTime.Now;

        await _userRepository.AddAsync(newUser);

        return _mapper.Map<UserDto>(newUser);
    }

    public async Task<UserDto> UpdateUser(UserDto user, BaseAuthDto newData)
    {
        var existingUser = await _userRepository.FindByEmailAsync(user.Email)
            ?? throw new Exception($"User with email: {user.Email} not found!");

        existingUser.Name = newData.Name;
        existingUser.LastLoginAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(existingUser);

        return _mapper.Map<UserDto>(existingUser);
    }
}
