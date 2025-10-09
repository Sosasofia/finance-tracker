using FinanceTracker.Application.Features.Categories;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;
[ApiController]
[Route("api/[controller]")]

public class CategoryController : BaseController
{
    private readonly ICategoryApplicationService _categoryService;

    public CategoryController(ICategoryApplicationService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> Categories()
    {
        var categories = await _categoryService.GetCategoriesAsync();

        if (!categories.Any())
        {
            return NotFound();
        }

        return Ok(categories);
    }

    [HttpGet("custom")]
    [Authorize(AuthenticationSchemes = "CustomJWT")]
    public async Task<ActionResult<IEnumerable<CustomCategory>>> CustomCategories()
    {
        if (!UserId(out var userGuid))
        {
            return Unauthorized("Missing or invalid user ID claim");
        }

        var customCategories = await _categoryService.GetCategoriesByUserIdAsync(userGuid);

        if (!customCategories.Any())
        {
            return NotFound();
        }

        return Ok(customCategories);
    }

    [HttpPost("custom")]
    [Authorize(AuthenticationSchemes = "CustomJWT")]
    public async Task<ActionResult<CustomCategory>> AddCustomCategory([FromBody] CustomCategoryDto categoryDTO)
    {
        if (!UserId(out var userGuid))
        {
            return Unauthorized("Missing or invalid user ID claim");
        }

        if (categoryDTO == null || string.IsNullOrWhiteSpace(categoryDTO.Name))
        {
            return BadRequest("Invalid custom category data.");
        }

        var createdCategory = await _categoryService.CreateCustomCategoryAsync(userGuid, categoryDTO);

        if (createdCategory == null)
        {
            return BadRequest("There was a problem creating custom category");
        }

        return Ok(createdCategory);
    }
}