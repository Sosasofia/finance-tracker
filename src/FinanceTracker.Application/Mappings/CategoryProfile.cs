using AutoMapper;
using FinanceTracker.Application.Features.Categories;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>();
    }
}
