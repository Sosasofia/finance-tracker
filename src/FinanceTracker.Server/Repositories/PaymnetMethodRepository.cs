using FinanceTracker.Server.Models;
using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Server.Repositories;

public class PaymnetMethodRepository : IPaymentMethodRepository
{
    private readonly Context _context;
    public PaymnetMethodRepository(Context context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods()
    {
        var paymentMethods = await _context.PaymentMethods.AsNoTracking().ToListAsync();

        return paymentMethods;
    }

    public async Task<bool> PaymentMethodExistsAsync(Guid paymentMethodId)
    {
        return await _context.PaymentMethods.AnyAsync(pm => pm.Id == paymentMethodId);
    }
}
