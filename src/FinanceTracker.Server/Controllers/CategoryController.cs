using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Categories;
using FinanceTracker.Domain.Entities;
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
    public async Task<ActionResult<IEnumerable<Category>>> Categories()
    {
        var userId = _currentUserService.UserId();

        var categories = await _categoryService.GetCategoriesAsync(userId);

        return !categories.Any() ? NotFound() : Ok(categories);
    }


    [HttpPost]
    public async Task<ActionResult<Category>> AddCategory([FromBody] CreateCategoryDto categoryDTO)
    {
        var userId = _currentUserService.UserId();

        if (categoryDTO == null || string.IsNullOrWhiteSpace(categoryDTO.Name))
        {
            return BadRequest("Invalid custom category data.");
        }

        var createdCategory = await _categoryService.CreateCategoryAsync(userId, categoryDTO);


        return createdCategory == null ? BadRequest("There was a problem creating custom category") : Ok(createdCategory);
    }
}
