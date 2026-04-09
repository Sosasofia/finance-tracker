using AutoMapper;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Application.Features.Users.Models;
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

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        return await _userRepository.ExistsByIdAsync(id);
    }

    public async Task<UserDto> GetByEmail(string email)
    {
        var user = await _userRepository.FindByEmailAsync(email);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateUser(BaseAuthDto authDto)
    {
        var newUser = _mapper.Map<User>(authDto);

        await _userRepository.AddAsync(newUser);

        return _mapper.Map<UserDto>(newUser);
    }

    public async Task<UserDto> UpdateUser(UserDto user, BaseAuthDto newData)
    {
        var existingUser = await _userRepository.FindByEmailAsync(user.Email)
            ?? throw new Exception($"User with email: {user.Email} not found!");

        existingUser.UpdateProfile(newData.Name, null);
        existingUser.RecordLogin();

        await _userRepository.UpdateAsync(existingUser);

        return _mapper.Map<UserDto>(existingUser);
    }
}
