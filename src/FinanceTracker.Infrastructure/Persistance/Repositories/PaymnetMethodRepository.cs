using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistance.Repositories;

public class PaymnetMethodRepository : IPaymentMethodRepository
{
    private readonly ApplicationDbContext _context;
    public PaymnetMethodRepository(ApplicationDbContext context)
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
