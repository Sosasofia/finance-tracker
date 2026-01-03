using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistance.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction> AddTransactionAsync(Transaction transaction)
    {
        var savedTransaction = await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        return savedTransaction.Entity;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(Guid userId)
    {
        var transactions = await _context.Transactions.Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .Include(t => t.InstallmentsList)
            .Include(t => t.Reimbursement)
            .ToListAsync();

        return transactions;
    }

    public async Task<Transaction> GetTransactionByIdAsync(Guid id)
    {
        return await _context.Transactions
                     .Where(t => t.Id == id)
                     .FirstOrDefaultAsync();
    }

    public async Task<Transaction> GetTransactionsByIdAndUserAsync(Guid transactionId, Guid userId)
    {
        return await _context.Transactions
                              .Where(t => t.Id == transactionId && t.UserId == userId)
                              .FirstOrDefaultAsync();
    }

    public async Task DeleteTransactionAsync(Transaction transaction)
    {
        if (transaction != null)
        {
            transaction.IsDeleted = true;
            transaction.DeletedAt = DateTime.UtcNow;

            //_context.Entry(transaction).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }

    public async Task<Transaction> RestoreDeleteTransactionAsync(Transaction transaction)
    {
        if (transaction != null && transaction.IsDeleted)
        {
            transaction.IsDeleted = false;
            transaction.DeletedAt = null;

            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return transaction;
        }

        return null;
    }

    public async Task<Transaction> GetTransactionByIdAndUserIncludingDeletedAsync(Guid transactionId, Guid userId)
    {
        return await _context.Transactions
                             .IgnoreQueryFilters()
                             .Where(t => t.Id == transactionId && t.UserId == userId)
                             .FirstOrDefaultAsync();
    }

    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        if (transaction != null)
        {
            _context.Entry(transaction).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Transaction>> GetByUserAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        var transactionsInRange = await _context.Transactions
                                                .Where(t => t.UserId == userId &&
                                                            t.Date <= endDate &&
                                                            t.Date >= startDate)
                                                .Include(t => t.Category)
                                                .Include(t => t.PaymentMethod)
                                                .OrderByDescending(t => t.Date)
                                                .ToListAsync();
        return transactionsInRange;
    }
}
