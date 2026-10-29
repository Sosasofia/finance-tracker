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

    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods(Guid userId, CancellationToken ct)
    {
        return await _context.PaymentMethods
            .Where(m => m.UserId == userId || m.UserId == null)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid paymentMethodId, CancellationToken ct)
    {
        return await _context.PaymentMethods.AnyAsync(pm => pm.Id == paymentMethodId, ct);
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, string name, CancellationToken ct)
    {
        return await _context.PaymentMethods.AnyAsync(cc => cc.UserId == userId && cc.Name == name, ct);
    }

    public async Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod, CancellationToken ct)
    {
        _context.PaymentMethods.Add(paymentMethod);
        await _context.SaveChangesAsync(ct);

        return paymentMethod;
    }

    public async Task<PaymentMethod?> GetByIdAsync(Guid paymentMethodId, Guid userId, CancellationToken ct)
    {
        return await _context.PaymentMethods
            .FirstOrDefaultAsync(p => p.Id == paymentMethodId &&
                                      (p.UserId == userId || p.UserId == null), ct);
    }

    public async Task<bool> IsInUseAsync(Guid paymentMethodId, CancellationToken ct)
    {
        return await _context.Transactions.AnyAsync(t => t.PaymentMethodId == paymentMethodId, ct);
    }

    public async Task DeleteAsync(PaymentMethod paymentMethod, CancellationToken ct)
    {
        _context.PaymentMethods.Remove(paymentMethod);
        await _context.SaveChangesAsync(ct);
    }
}
