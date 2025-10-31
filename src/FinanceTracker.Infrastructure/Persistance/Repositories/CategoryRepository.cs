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

    public async Task<IEnumerable<Category>> GetCategories(Guid userId)
    {
        return await _context.Categories
            .Where(c => c.UserId == userId || c.UserId == null)
            .ToListAsync();
    }

    public async Task<bool> CategoryExistsAsync(Guid categoryId)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId);
    }

    public async Task<bool> ExistsCategoryForUserAsync(Guid userId, string name)
    {
        return await _context.Categories.AnyAsync(cc => cc.UserId == userId && cc.Name.Equals(name));
    }

    public async Task<Category> AddCategoryAsync(Category newCategory)
    {
        _context.Categories.Add(newCategory);
        await _context.SaveChangesAsync();

        return newCategory;
    }
}
