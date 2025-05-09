using FinanceTracker.Server.Models;
using FinanceTracker.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Server.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly Context _context;

        public TransactionRepository(Context context)
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
                .Include(t => t.InstallmentsList)
                .Include(t => t.Reimbursement)
                .ToListAsync();

            return transactions;
        }

        public async Task<Transaction> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteTransactionAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
