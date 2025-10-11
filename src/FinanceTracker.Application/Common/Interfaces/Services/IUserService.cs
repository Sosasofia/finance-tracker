using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface IUserService
{
    Task<bool> ExistsAsync(Guid id);
    Task<User> FindOrCreateUserAsync(string email, string? name, string? pictureUrl); 
}
