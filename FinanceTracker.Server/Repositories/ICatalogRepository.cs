using FinanceTracker.Server.Models.Entities;

namespace FinanceTracker.Server.Repositories
{
    public interface ICatalogRepository
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<PaymentMethod>> GetPaymentMethods();
        Task<bool> CategoryExistsAsync(Guid categoryId);
        Task<bool> PaymentMethodExistsAsync(Guid categoryName);
    }
}
