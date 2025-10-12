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

    public async Task<User> ProcessGoogleLoginAsync(string email, string name, string pictureUrl)
    {
        var user = await _userRepository.FindByEmailAsync(email);

        if (user == null)
        {
            user = new User
            {
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
            user.Name = name;
            user.ProfilePictureUrl = pictureUrl;
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        return user;
    }
}
