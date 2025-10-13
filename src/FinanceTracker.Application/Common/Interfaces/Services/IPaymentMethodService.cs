using FinanceTracker.Application.Features.PaymentMethods;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface IPaymentMethodService
{
    /// <summary>
    /// Asynchronously retrieves a collection of available payment methods.
    /// </summary>
    /// <remarks>This method is typically used to fetch payment methods for display in a user interface or 
    /// for processing transactions. The caller should handle the case where the returned collection  is
    /// empty.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains an  IEnumerable{T} of
    /// PaymentMethodDto objects, where each object  represents a payment method. The collection will be empty if no
    /// payment methods are available.</returns>
    Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync();
}
