using FinanceTracker.Application.Features.Transactions;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface IFileGenerator
{
    byte[] GenerateExcel(IEnumerable<TransactionExportDto> data);
    byte[] GenerateCsv(IEnumerable<TransactionExportDto> data);
}
