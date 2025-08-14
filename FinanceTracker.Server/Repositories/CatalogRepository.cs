using FinanceTracker.Server.Models;
using FinanceTracker.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Server.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly Context _context;

        public CatalogRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();

            return categories;
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods()
        {
            var paymentMethods = await _context.PaymentMethods.AsNoTracking().ToListAsync();

            return paymentMethods;
        }

        public async Task<bool> CategoryExistsAsync(Guid categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.Id == categoryId);
        }
        public async Task<bool> PaymentMethodExistsAsync(Guid paymentMethodId)
        {
            return await _context.PaymentMethods.AnyAsync(pm => pm.Id == paymentMethodId);
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
                .AsNoTracking()
                .ToListAsync();
            return customCategories;
        }
    }
}
