using FinanceTracker.Server.Models.Entities;

namespace FinanceTracker.Server.Repositories
{
    public interface ICatalogRepository
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<PaymentMethod>> GetPaymentMethods();
    }
}
