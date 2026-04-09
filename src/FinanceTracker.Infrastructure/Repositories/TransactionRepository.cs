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

    public async Task<Transaction?> GetTransactionByIdAsync(Guid id)
    {
        return await _context.Transactions
            .Include(t => t.Installments)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Transaction>> GetByUserAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        t.Date >= startDate &&
                        t.Date <= endDate)
            .Include(t => t.Category)
            .Include(t => t.PaymentMethod)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionsByIdAndUserAsync(Guid transactionId, Guid userId)
    {
        return await _context.Transactions
            .Include(t => t.Installments)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);
    }

    public async Task<Transaction> AddTransactionAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTransactionAsync(Transaction transaction)
    {
        transaction.SoftDelete();
        await _context.SaveChangesAsync();
    }

    public async Task<Transaction> RestoreDeleteTransactionAsync(Transaction transaction)
    {
        transaction.Restore();
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(Guid userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .Include(t => t.Installments)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAndUserIncludingDeletedAsync(Guid transactionId, Guid userId)
    {
        return await _context.Transactions
            .IgnoreQueryFilters()
            .Include(t => t.Installments)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);
    }
}
