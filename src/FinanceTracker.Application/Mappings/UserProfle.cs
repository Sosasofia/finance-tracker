using AutoMapper;
using FinanceTracker.Application.Features.Users;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
    }
}
