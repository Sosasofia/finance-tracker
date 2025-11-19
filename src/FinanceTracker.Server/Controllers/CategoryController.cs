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

    /// <summary>Retrieves all available categories, including global categories and user-specific categories.</summary>
    /// <remarks>
    /// <p><strong>Description:</strong> Returns the complete set of categories accessible to the authenticated user.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    /// <li><strong>Global categories:</strong> Categories that are available to all users by default.</li>
    /// <li><strong>User categories:</strong> Categories created specifically by the authenticated user.</li>
    /// <li>The returned collection includes both types, with no distinction required on the client side unless desired.</li>
    /// <li>Requires a valid authentication token.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong> On success, the endpoint returns <c>200 OK</c> with the combined collection of categories.</p>
    /// </remarks>
    /// <response code="200">Returns a list of global and user-specific categories.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> Categories()
    {
        var userId = _currentUserService.UserId();

        var categories = await _categoryService.GetCategoriesAsync(userId);

        return Ok(categories);
    }

    /// <summary>Retrieves a specific category by its unique identifier.</summary>
    /// <remarks>
    /// <p><strong>Description:</strong> Returns the category that matches the provided ID.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>If the category does not exist, the server returns <c>404 Not Found</c>.</li>
    ///   <li>Ownership validation should be implemented in the service layer.</li>
    /// </ul>
    /// <p><strong>Result:</strong>  
    /// <c>200 OK</c> with the matched category when found.</p>
    /// </remarks>
    /// <param name="id">Unique identifier of the category.</param>
    /// <response code="200">Returns the category when found.</response>
    /// <response code="404">Returned when no category exists with the provided ID.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategoryById([FromRoute] Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        return Ok(category);
    }

    /// <summary>Adds a new category and associates it with the user performing the request.</summary>
    /// <remarks>
    /// <p><strong>Description:</strong> Adds a new category associated with the user making the request.</p>
    ///     
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>Validates the incoming payload.</li>
    ///   <li>Ensures the category is linked to the authenticated user.</li>
    ///   <li>On success, the response contains a <strong>Location</strong> header.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>201 Created</c> containing the newly created category.</p>
    /// </remarks>
    /// <param name="categoryDTO">Data needed to create the category.</param>
    /// <response code="201">Returns the created category and a URL to retrieve it via the <strong>Location</strong> header.</response>
    /// <response code="400">Returned when the request fails validation.</response>
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

    /// <summary>Deletes a category associated with the authenticated user.</summary>
    /// <remarks>
    /// <p><strong>Description:</strong> Deletes a category owned by the authenticated user.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>Deletion only succeeds if the category belongs to the authenticated user.</li>
    ///   <li>If the category does not exist, a <c>404 Not Found</c> response is returned.</li>
    ///   <li>Successful deletion returns no body.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>204 No Content</c> when the category is removed successfully.</p>
    /// </remarks>
    /// <param name="id">Unique identifier of the category to delete.</param>
    /// <response code="204">Indicates the category was successfully deleted.</response>
    /// <response code="404">Returned when the category does not exist.</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        var userId = _currentUserService.UserId();

        await _categoryService.DeleteAsync(userId, id);

        return NoContent();
    }
}
