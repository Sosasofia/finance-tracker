using FinanceTracker.Application.Common.DTOs;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiConventionType(typeof(DefaultApiConventions))]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)] 
[ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)] 
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ICurrentUserService _currentUserService;

    public CategoryController(ICategoryService categoryService, ICurrentUserService currentUserService)
    {
        _categoryService = categoryService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets all categories for the currently authenticated user.
    /// </summary>
    /// <returns>A list of the user's categories.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> Categories()
    {
        var userId = _currentUserService.UserId();

        var categories = await _categoryService.GetCategoriesAsync(userId);

        return Ok(categories);
    }

    /// <summary>
    /// Gets a specific category by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <returns>The specified category.</returns>
    /// <response code="200">Returns the specified category.</response>
    /// <response code="404">If the category with the specified ID is not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategoryById([FromRoute] Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        return Ok(category);
    }

    /// <summary>
    /// Creates a new category for the currently authenticated user.
    /// </summary>
    /// <param name="categoryDTO">The details for the new category.</param>
    /// <returns>The newly created category.</returns>
    /// <response code="201">Returns the newly created category and a link to retrieve it.</response>
    /// <response code="400">If the request payload is invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Deletes a specific category belonging to the current user.
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <response code="204">Indicates the category was successfully deleted.</response>
    /// <response code="404">If the category with the specified ID is not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        var userId = _currentUserService.UserId();

        await _categoryService.DeleteAsync(userId, id);

        return NoContent();
    }
}
