using AutoMapper;

using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Repositories;

namespace FinanceTracker.Server.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IUserService userService, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            try
            {
                var categories = await  _categoryRepository.GetCategories();

                return categories;
            } 
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving categories", ex);
            }
        }

        public async Task<IEnumerable<CustomCategory>> GetCategoriesByUserIdAsync(Guid userId)
        {

            if (!await _userService.ExistsAsync(userId))
            {
                throw new Exception($"User with id: {userId} does not exists");
            }

            try
            {
                var categories = await _categoryRepository.GetCustomCategoriesAsync(userId);

                return categories;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error retrieving categories for user with id: {userId}", ex);
            }
        }
    
        public async Task<CustomCategory> CreateCustomCategoryAsync(Guid userId, CustomCategoryDTO categoryDTO)
        {
            var customCategoryExists = await _categoryRepository.CategoryExistsForUserAsync(userId, categoryDTO.Name);

            if (customCategoryExists)
            {
                throw new Exception("A custom category with this name already exists for this user.");
            }

            var newCategory = _mapper.Map<CustomCategory>(categoryDTO);
            newCategory.UserId = userId;

            try
            {
                var createdCategory = await _categoryRepository.AddCustomCategoryAsync(newCategory);

                return createdCategory;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
    }
}
