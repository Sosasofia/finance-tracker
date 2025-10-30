using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Transactions;
using MiniExcelLibs;

namespace FinanceTracker.Infrastructure.Files;

public class MiniExcelFileGenerator : IFileGenerator
{
    public byte[] GenerateCsv(IEnumerable<TransactionExportDto> data)
    {
        using var stream = new MemoryStream();
        MiniExcel.SaveAs(stream, data, excelType: ExcelType.CSV);
        return stream.ToArray();
    }

    public byte[] GenerateExcel(IEnumerable<TransactionExportDto> data)
    {
        using var stream = new MemoryStream();
        MiniExcel.SaveAs(stream, data, excelType: ExcelType.XLSX);
        return stream.ToArray();
    }
}
