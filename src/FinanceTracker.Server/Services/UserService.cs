using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Repositories;
using FinanceTracker.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Server.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository , ApplicationDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        try
        {
            return await _userRepository.ExistsAsync(id);
        } 
        catch(Exception ex) 
        {
                throw new Exception();
        }
    }

    public async Task<User> FindOrCreateUserAsync(string? email, string? name, string? pictureUrl)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

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

            _context.Users.Add(user);
        }
        else
        {
            user.LastLoginAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return user;
    }
}
