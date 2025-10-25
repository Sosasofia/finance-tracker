using AutoMapper;
using FinanceTracker.Application.Features.Auth;
using FinanceTracker.Application.Features.Users;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<BaseAuthDto, User>();
        CreateMap<PasswordAuthDto, User>();
        CreateMap<GoogleAuthDto, User>();
        CreateMap<GoogleAuthDto, GoogleTokenPayload>();
        CreateMap<UserDto, UserResponse>();
    }
}
