using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetCategories(Guid userId, CancellationToken ct)
    {
        return await _context.Categories
            .Where(c => c.UserId == userId || c.UserId == null)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid categoryId, CancellationToken ct)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId, ct);
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, string name, CancellationToken ct)
    {
        return await _context.Categories.AnyAsync(cc => cc.UserId == userId && cc.Name == name, ct);
    }

    public async Task<Category> AddAsync(Category newCategory, CancellationToken ct)
    {
        _context.Categories.Add(newCategory);
        await _context.SaveChangesAsync(ct);

        return newCategory;
    }

    public async Task<Category?> GetByIdAsync(Guid categoryId, Guid userId, CancellationToken ct)
    {
        return await _context.Categories
        .FirstOrDefaultAsync(c => c.Id == categoryId &&
                                 (c.UserId == userId || c.UserId == null), ct);
    }

    public async Task<bool> IsInUseAsync(Guid categoryId, CancellationToken ct)
    {
        return await _context.Transactions.AnyAsync(t => t.CategoryId == categoryId, ct);
    }

    public async Task DeleteAsync(Category category, CancellationToken ct)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(ct);
    }
}
