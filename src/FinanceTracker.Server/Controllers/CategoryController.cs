using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]

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
        var userId = _currentUserService.UserId();

        var categories = await _categoryService.GetCategoriesAsync(userId);

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> AddCategory([FromBody] CreateCategoryDto categoryDTO)
    {
        var userId = _currentUserService.UserId();

        var createdCategory = await _categoryService.CreateAsync(userId, categoryDTO);

        return CreatedAtAction(
            nameof(GetCategoryById),
            new { id = createdCategory.Id },
            createdCategory);
        ;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var userId = _currentUserService.UserId();

        await _categoryService.DeleteAsync(userId, id);

        return NoContent();
    }
}
