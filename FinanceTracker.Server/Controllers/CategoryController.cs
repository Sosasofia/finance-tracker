using AutoMapper;
using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CategoryController : BaseController
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper) 
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetCategories();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("custom")]
        [Authorize(AuthenticationSchemes = "CustomJWT")]
        public async Task<ActionResult<IEnumerable<CustomCategory>>> GetCustomCategories()
        {
            if (!UserId(out var userGuid))
            {
                return Unauthorized("Missing or invalid user ID claim");
            }
            try
            {
                var customCategories = await _categoryRepository.GetCustomCategoriesAsync(userGuid);
                return Ok(customCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("custom")]
        [Authorize(AuthenticationSchemes = "CustomJWT")]
        public async Task<ActionResult<CustomCategory>> AddCustomCategory([FromBody] CustomCategoryDTO categoryDTO)
        {
            if (!UserId(out var userGuid))
            {
                return Unauthorized("Missing or invalid user ID claim");
            }
            if (categoryDTO == null || string.IsNullOrWhiteSpace(categoryDTO.Name))
            {
                return BadRequest("Invalid custom category data.");
            }

            // Check if the category already exists for the user
            var existingCategory = await _categoryRepository.GetCustomCategoriesAsync(userGuid);
            if (existingCategory.Any(c => c.Name.Equals(categoryDTO.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("A custom category with this name already exists for this user.");
            }

            var newCategory = _mapper.Map<CustomCategory>(categoryDTO);
            newCategory.UserId = userGuid;

            try
            {
                var createdCategory = await _categoryRepository.AddCustomCategoryAsync(newCategory);
                return CreatedAtAction(nameof(GetCustomCategories), new { id = createdCategory.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
