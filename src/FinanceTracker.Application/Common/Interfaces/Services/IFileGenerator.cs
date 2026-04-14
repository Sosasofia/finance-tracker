using FinanceTracker.Application.Features.Transactions.Models;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface IFileGenerator
{
    Task<byte[]> GenerateExcelAsync(IEnumerable<TransactionExportDto> data, CancellationToken ct);
    Task<byte[]> GenerateCsvAsync(IEnumerable<TransactionExportDto> data, CancellationToken ct);
}
