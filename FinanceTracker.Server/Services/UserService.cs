using FinanceTracker.Server.Models;
using FinanceTracker.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Server.Services
{
    public class UserService : IUserService
    {
        private readonly Context _context;

        public UserService(Context context)
        {
            _context = context;
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
}
