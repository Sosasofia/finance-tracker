using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public class UserApplicationService : IUserApplicationService
{
    private readonly IUserRepository _userRepository;

    public UserApplicationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
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

    public async Task<User> FindOrCreateUserAsync(string? email, string? name, string? pictureUrl)
    {
        //var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        var user = await _userRepository.ExistsByEmailAsync(email);

        
        if (user == null)
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

        }
        else
        {
            user.LastLoginAt = DateTime.UtcNow;
        }

        await _userRepository.SaveChangesAsync();

        return user;
    }
}

