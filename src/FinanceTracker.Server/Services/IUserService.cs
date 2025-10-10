using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Server.Services
{
    public interface IUserService
    {
        Task<bool> ExistsAsync(Guid id);
        Task<User> FindOrCreateUserAsync(string email, string? name, string? pictureUrl); 
    }
}
