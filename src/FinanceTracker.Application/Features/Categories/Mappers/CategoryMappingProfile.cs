using AutoMapper;
using FinanceTracker.Application.Features.Categories.Models;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Features.Categories.Mappers;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>();
    }
}
