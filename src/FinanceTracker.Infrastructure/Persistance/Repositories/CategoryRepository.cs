using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistance.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        var categories = await _context.Categories.ToListAsync();

        return categories;
    }

    public async Task<bool> CategoryExistsAsync(Guid categoryId)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId);
    }

    public async Task<bool> CategoryExistsForUserAsync(Guid userId, string name)
    {
        return await _context.CustomCategories.AnyAsync(cc => cc.UserId == userId && cc.Name.Equals(name));
    }

    public async Task<CustomCategory> AddCustomCategoryAsync(CustomCategory customCategory)
    {
        _context.CustomCategories.Add(customCategory);
        await _context.SaveChangesAsync();

        return customCategory;
    }

    public async Task<IEnumerable<CustomCategory>> GetCustomCategoriesAsync(Guid userId)
    {
        var customCategories = await _context.CustomCategories
            .Where(cc => cc.UserId == userId)
            .ToListAsync();

        return customCategories;
    }
}
