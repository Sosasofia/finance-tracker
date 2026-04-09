using AutoMapper;
using FinanceTracker.Application.Features.Auth.Models;
using FinanceTracker.Application.Features.Users.Models;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Features.Auth.Mappers;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<User, UserResponse>();
        CreateMap<BaseAuthDto, User>();
        CreateMap<PasswordAuthDto, User>();
        CreateMap<GoogleAuthDto, User>();
        CreateMap<GoogleAuthDto, GoogleTokenPayload>();
        CreateMap<UserDto, UserResponse>();
    }
}
