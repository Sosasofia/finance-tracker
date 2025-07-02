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
    }
}
