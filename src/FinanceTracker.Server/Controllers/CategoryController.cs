using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Features.Categories.Commands.CreateCategory;
using FinanceTracker.Application.Features.Categories.Commands.DeleteCategory;
using FinanceTracker.Application.Features.Categories.Models;
using FinanceTracker.Application.Features.Categories.Queries.GetCategories;
using FinanceTracker.Application.Features.Categories.Queries.GetCategoryById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Authorize]
[EnableRateLimiting("fixed")]
[Route("api/categories")]
[Produces("application/json")]
[Consumes("application/json")]
public class CategoryController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly GetCategoriesQueryHandler _getListHandler;
    private readonly GetCategoryByIdQueryHandler _getByIdHandler;
    private readonly CreateCategoryCommandHandler _createHandler;
    private readonly DeleteCategoryCommandHandler _deleteHandler;

    public CategoryController(
        ICurrentUserService currentUserService,
        GetCategoriesQueryHandler getListHandler,
        GetCategoryByIdQueryHandler getByIdHandler,
        CreateCategoryCommandHandler createHandler,
        DeleteCategoryCommandHandler deleteHandler)
    {
        _currentUserService = currentUserService;
        _getListHandler = getListHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _deleteHandler = deleteHandler;
    }

    /// <summary>
    /// Retrieves the list of categories available to the current user.
    /// </summary>
    /// <remarks>This endpoint is accessible via HTTP GET. The returned list is filtered based on the current
    /// user's context.</remarks>
    /// <returns>An asynchronous operation that returns an <see cref="ActionResult{T}"/> containing a collection of <see
    /// cref="CategoryDto"/> objects representing the available categories.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var query = new GetCategoriesQuery(_currentUserService.UserId());
        var categories = await _getListHandler.Handle(query, default);

        return Ok(categories);
    }

    /// <summary>
    /// Retrieves the category with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category to retrieve.</param>
    /// <returns>An ActionResult containing the category data if found; otherwise, a NotFound result.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById([FromRoute] Guid id)
    {
        var query = new GetCategoryByIdQuery(id, _currentUserService.UserId());
        var category = await _getByIdHandler.Handle(query, default);

        return Ok(category);
    }

    /// <summary>
    /// Creates a new category using the specified command and returns the created category.
    /// </summary>
    /// <param name="command">The command containing the details required to create the category. Must not be null.</param>
    /// <returns>An ActionResult containing the created category data. Returns a 201 Created response with the category details
    /// if successful.</returns>
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryCommand command)
    {
        var commandWithUser = command with { UserId = _currentUserService.UserId() };
        var createdCategory = await _createHandler.Handle(commandWithUser, default);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdCategory.Id },
            createdCategory);
    }

    /// <summary>
    /// Deletes the category with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <returns>An IActionResult indicating the result of the delete operation. Returns NoContent if the category was
    /// successfully deleted.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteCategoryCommand(id, _currentUserService.UserId());
        await _deleteHandler.Handle(command, default);

        return NoContent();
    }
}
