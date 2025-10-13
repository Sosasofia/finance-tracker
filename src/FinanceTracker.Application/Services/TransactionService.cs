using AutoMapper;
using FinanceTracker.Application.Common.DTOs;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Transactions;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Repositories;
using FluentValidation;


namespace FinanceTracker.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IInstallmentService _installmentService;
    private readonly IValidator<CreateTransactionDto> _createTransactionValidator;
    private readonly IMapper _mapper;

    public TransactionService(
        ITransactionRepository transactionRepository,
        IInstallmentService installmentService,
        IValidator<CreateTransactionDto?> createTransactionValidator,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _installmentService = installmentService;
        _createTransactionValidator = createTransactionValidator;
        _mapper = mapper;
    }

    public async Task<Result<TransactionResponse>> AddTransactionAsync(CreateTransactionDto transactionCreateDTO, Guid userID)
    {
        try
        {
            await _createTransactionValidator.ValidateAndThrowAsync(transactionCreateDTO);

            var transaction = _mapper.Map<Transaction>(transactionCreateDTO);
            transaction.UserId = userID;

            if (transactionCreateDTO.IsCreditCardPurchase)
            {
                transaction.InstallmentsList = _installmentService.GenerateInstallments(transactionCreateDTO);
            }

            var result = await _transactionRepository.AddTransactionAsync(transaction);
            var mappedResult = _mapper.Map<TransactionResponse>(result);

            return Result<TransactionResponse>.Success(mappedResult);
        }
        catch (ValidationException ex)
        {
            var errorMessages = ex.Errors
                                  .Select(e => e.ErrorMessage)
                                  .ToList();

            return Result<TransactionResponse>.Failure(errorMessages);
        }
    }

    /// <summary>
    /// Gets all transactions for a user by id.
    /// </summary>
    public async Task<IEnumerable<TransactionResponse>> GetTransactionsByUserAsync(Guid userId)
    {
        var transactions = await _transactionRepository.GetTransactionsByUserAsync(userId);

        var res = _mapper.Map<IEnumerable<Transaction>, IEnumerable<TransactionResponse>>(transactions);

        return res;
    }

    public async Task<bool> DeleteTransactionAsync(Guid transactionId, Guid userId)
    {
        var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(transactionId, userId);

        if (transaction == null)
        {
            throw new UnauthorizedAccessException("Denied access. Transaction not found or does not belong to the user.");
        }

        await _transactionRepository.DeleteTransactionAsync(transaction);

        return true;
    }

    public async Task<TransactionResponse> GetTransactionByIdAndUserAsync(Guid transactionId, Guid userId)
    {
        var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(transactionId, userId);

        if (transaction == null)
        {
            throw new UnauthorizedAccessException("Transaction not found or denied access.");
        }

        return _mapper.Map<TransactionResponse>(transaction);
    }

    public async Task<TransactionResponse> RestoreDeleteTransactionAsync(Guid transactionId, Guid userId)
    {
        var transaction = await _transactionRepository.GetTransactionByIdAndUserIncludingDeletedAsync(transactionId, userId);

        if (transaction == null)
        {
            throw new UnauthorizedAccessException("Transaction not found or denied access.");
        }
        if (!transaction.IsDeleted)
        {
            throw new UnauthorizedAccessException("Transaction is not deleted.");
        }

        var restoredTransaction = await _transactionRepository.RestoreDeleteTransactionAsync(transaction);

        return _mapper.Map<TransactionResponse>(transaction);
    }

    public async Task<Response<TransactionResponse>> UpdateTransactionAsync(Guid transactionId, UpdateTransactionDto transactionUpdateDTO, Guid userId)
    {
        var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(transactionId, userId);

        if (transaction == null)
        {
            throw new UnauthorizedAccessException("Transaction not found or denied access.");
        }

        _mapper.Map(transactionUpdateDTO, transaction);

        transaction.LastModifiedAt = DateTime.UtcNow;

        await _transactionRepository.UpdateTransactionAsync(transaction);

        return new Response<TransactionResponse>(_mapper.Map<TransactionResponse>(transaction));
    }
}
