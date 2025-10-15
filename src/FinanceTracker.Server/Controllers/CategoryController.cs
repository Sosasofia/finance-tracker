using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Categories;
using FinanceTracker.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]

public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ICurrentUserService _currentUserService;

    public CategoryController(ICategoryService categoryService, ICurrentUserService currentUserService)
    {
        _categoryService = categoryService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> Categories()
    {
        var categories = await _categoryService.GetCategoriesAsync();

        if (!categories.Any())
        {
            return NotFound();
        }

        return Ok(categories);
    }

    [HttpGet("custom")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CustomCategory>>> CustomCategories()
    {
        var userId = _currentUserService.UserId();

        if(userId == null)
        {
            return Unauthorized("Missing or invalid user ID claim");
        }

        var customCategories = await _categoryService.GetCategoriesByUserIdAsync(userId.Value);

        if (!customCategories.Any())
        {
            return NotFound();
        }

        return Ok(customCategories);
    }

    [HttpPost("custom")]
    [Authorize]
    public async Task<ActionResult<CustomCategory>> AddCustomCategory([FromBody] CustomCategoryDto categoryDTO)
    {
        var userId = _currentUserService.UserId();
        if (userId == null)
        {
            return Unauthorized("Missing or invalid user ID claim");
        }

        if (categoryDTO == null || string.IsNullOrWhiteSpace(categoryDTO.Name))
        {
            return BadRequest("Invalid custom category data.");
        }

        var createdCategory = await _categoryService.CreateCustomCategoryAsync(userId.Value, categoryDTO);

        if(createdCategory == null)
        {
            return BadRequest("There was a problem creating custom category");
        }

        return Ok(createdCategory);
    }
}
