using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Transactions.Models;
using MiniExcelLibs;

namespace FinanceTracker.Infrastructure.Files;

public class MiniExcelFileGenerator : IFileGenerator
{
    public async Task<byte[]> GenerateCsvAsync(IEnumerable<TransactionExportDto> data, CancellationToken ct)
    {
        using var stream = new MemoryStream();
        await MiniExcel.SaveAsAsync(stream, data, excelType: ExcelType.CSV, cancellationToken: ct);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerateExcelAsync(IEnumerable<TransactionExportDto> data, CancellationToken ct)
    {
        using var stream = new MemoryStream();
        await MiniExcel.SaveAsAsync(stream, data, excelType: ExcelType.XLSX, cancellationToken: ct);
        return stream.ToArray();
    }
}
