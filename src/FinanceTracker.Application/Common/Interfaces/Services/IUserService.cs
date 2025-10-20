using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Application.Features.Users;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface IUserService
{
    Task<bool> ExistsByAsync(Guid id);
    Task<UserDto> GetByEmail(string email);
    Task<UserDto> CreateUser(BaseAuthDto authDto);
    Task<UserDto> UpdateUser(UserDto user, BaseAuthDto googlePayload);
}
