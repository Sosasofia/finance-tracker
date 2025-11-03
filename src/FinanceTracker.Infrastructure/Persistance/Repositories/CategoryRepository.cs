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

    public async Task<bool> ExistsAsync(Guid categoryId)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId);
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, string name)
    {
        return await _context.Categories.AnyAsync(cc => cc.UserId == userId && cc.Name.Equals(name));
    }

    public async Task<Category> AddAsync(Category newCategory)
    {
        _context.Categories.Add(newCategory);
        await _context.SaveChangesAsync();

        return newCategory;
    }

    public async Task<Category?> GetByIdAsync(Guid categoryId)
    {
        return await _context.Categories.FindAsync(categoryId);
    }

    public async Task<bool> IsInUseAsync(Guid categoryId)
    {
        return await _context.Transactions.AnyAsync(t => t.CategoryId == categoryId);
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}
