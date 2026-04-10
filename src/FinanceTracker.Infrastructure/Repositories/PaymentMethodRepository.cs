using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentMethodRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods(Guid userId)
    {
        return await _context.PaymentMethods
            .Where(m => m.UserId == userId || m.UserId == null)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid paymentMethodId)
    {
        return await _context.PaymentMethods.AnyAsync(pm => pm.Id == paymentMethodId);
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, string name)
    {
        return await _context.PaymentMethods.AnyAsync(cc => cc.UserId == userId && cc.Name.Equals(name));
    }

    public async Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod)
    {
        _context.PaymentMethods.Add(paymentMethod);
        await _context.SaveChangesAsync();

        return paymentMethod;
    }

    public async Task<PaymentMethod?> GetByIdAsync(Guid paymentMethodId, Guid userId)
    {
        return await _context.PaymentMethods
            .FirstOrDefaultAsync(p => p.Id == paymentMethodId && 
                                      (p.UserId == userId || p.UserId == null));
    }

    public async Task<bool> IsInUseAsync(Guid paymentMethodId)
    {
        return await _context.Transactions.AnyAsync(t => t.PaymentMethodId == paymentMethodId);
    }

    public async Task DeleteAsync(PaymentMethod paymentMethod)
    {
        _context.PaymentMethods.Remove(paymentMethod);

        await _context.SaveChangesAsync();
    }
}
