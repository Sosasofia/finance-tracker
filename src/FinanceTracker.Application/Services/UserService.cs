using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        try
        {
            return await _userRepository.ExistsByIdAsync(id);
        } 
        catch(Exception ex) 
        {
            throw new Exception();
        }
    }

    public async Task<User> FindOrCreateUserAsync(string? email, string? name, string? pictureUrl)
    {
        var user = await _userRepository.ExistsByEmailAsync(email);

        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = name,
                ProfilePictureUrl = pictureUrl,
                Provider = "google",
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
        }
        else
        {
            user.LastLoginAt = DateTime.UtcNow;
        }

        await _userRepository.SaveChangesAsync();

        return user;
    }
}
