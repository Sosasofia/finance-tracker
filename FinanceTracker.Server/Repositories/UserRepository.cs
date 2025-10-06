using FinanceTracker.Server.Models;

using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Server.Repositories
{
    public class UserRepository :IUserRepository
    {
        private readonly Context _context;
        public UserRepository(Context context) 
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }
    }
}
