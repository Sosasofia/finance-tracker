using FinanceTracker.Server.Models.Entities;

namespace FinanceTracker.Server.Services
{
    public interface IUserService
    {
        Task<User> FindOrCreateUserAsync(string email, string? name, string? pictureUrl);
    }
}
