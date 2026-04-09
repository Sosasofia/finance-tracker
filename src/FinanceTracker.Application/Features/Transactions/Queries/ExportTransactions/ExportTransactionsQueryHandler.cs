using AutoMapper;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Transactions.Queries.ExportTransactions;

public class ExportTransactionsQueryHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IFileGenerator _fileGeneratorService;
    private readonly IMapper _mapper;

    public ExportTransactionsQueryHandler(
        ITransactionRepository transactionRepository,
        IFileGenerator fileGeneratorService,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _fileGeneratorService = fileGeneratorService;
        _mapper = mapper;
    }

    public async Task<byte[]> Handle(ExportTransactionsQuery query, CancellationToken ct)
    {
        var startDate = query.Start.Date;
        var endDate = query.End.Date.AddDays(1).AddTicks(-1);

        var transactions = await _transactionRepository.GetByUserAndDateRangeAsync(
            query.UserId,
            startDate,
            endDate);

        var mappedToExport = _mapper.Map<IEnumerable<TransactionExportDto>>(transactions);

        return query.Format switch
        {
            ExportFormat.Excel => _fileGeneratorService.GenerateExcel(mappedToExport),
            ExportFormat.Csv => _fileGeneratorService.GenerateCsv(mappedToExport),
            _ => throw new ArgumentOutOfRangeException(nameof(query.Format), "Format not supported")
        };
    }
}
