using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public sealed class ReceiptRepository : IReceiptRepository
{
    private readonly ApplicationDbContext _context;

    public ReceiptRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Receipt receipt, CancellationToken ct)
    {
        await _context.Receipts.AddAsync(receipt, ct);
    }

    public Task<Receipt?> GetByIdAndUserAsync(Guid receiptId, Guid userId, CancellationToken ct)
    {
        return _context.Receipts.FirstOrDefaultAsync(
            receipt => receipt.Id == receiptId && receipt.UserId == userId,
            ct);
    }

    public async Task<IReadOnlyList<Receipt>> GetByUserAsync(Guid userId, CancellationToken ct)
    {
        return await _context.Receipts
            .AsNoTracking()
            .Where(receipt => receipt.UserId == userId)
            .OrderByDescending(receipt => receipt.UploadedAt)
            .ToListAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
