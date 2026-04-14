using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetTransactionByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Transactions
            .Include(t => t.Installments)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<IEnumerable<Transaction>> GetByUserAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        t.Date >= startDate &&
                        t.Date <= endDate)
            .Include(t => t.Category)
            .Include(t => t.PaymentMethod)
            .OrderByDescending(t => t.Date)
            .ToListAsync(ct);
    }

    public async Task<Transaction?> GetTransactionsByIdAndUserAsync(Guid transactionId, Guid userId, CancellationToken ct)
    {
        return await _context.Transactions
            .Include(t => t.Installments)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId, ct);
    }

    public async Task<Transaction> AddTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync(ct);

        return transaction;
    }

    public async Task UpdateTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        transaction.SoftDelete();
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Transaction> RestoreDeleteTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        transaction.Restore();
        await _context.SaveChangesAsync(ct);

        return transaction;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(Guid userId, CancellationToken ct)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .Include(t => t.Installments)
            .OrderByDescending(t => t.Date)
            .ToListAsync(ct);
    }

    public async Task<Transaction?> GetTransactionByIdAndUserIncludingDeletedAsync(Guid transactionId, Guid userId, CancellationToken ct)
    {
        return await _context.Transactions
            .IgnoreQueryFilters()
            .Include(t => t.Installments)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId, ct);
    }
}
