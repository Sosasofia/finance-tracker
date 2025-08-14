using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Entities;

namespace FinanceTracker.Server.Repositories
{
    public interface ICatalogRepository
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<PaymentMethod>> GetPaymentMethods();
        Task<bool> CategoryExistsAsync(Guid categoryId);
        Task<bool> PaymentMethodExistsAsync(Guid categoryName);
        Task<CustomCategory> AddCustomCategoryAsync(CustomCategory customCategoryDTO);
        Task<IEnumerable<CustomCategory>> GetCustomCategoriesAsync(Guid userId);
    }
}
