using AutoMapper;
using FinanceTracker.Application.Common.DTOs;
using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Domain.ValueObjects;
using FluentValidation;

namespace FinanceTracker.Application.Features.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateTransactionCommand> _validator;
    private readonly IMapper _mapper;

    public CreateTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        IPaymentMethodRepository paymentMethodRepository,
        ICurrentUserService currentUserService,
        IValidator<CreateTransactionCommand> validator,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _currentUserService = currentUserService;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<TransactionResponse>> Handle(CreateTransactionCommand command, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(command, ct);

        var category = await _categoryRepository.GetByIdAsync(command.CategoryId!)
            ?? throw new NotFoundException("Category", command.CategoryId);

        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(command.PaymentMethodId!)
            ?? throw new NotFoundException("Payment Method", command.PaymentMethodId);

        var amount = Money.Create(command.Amount);
        var userId = _currentUserService.UserId();
        var utcDate = command.Date.Kind == DateTimeKind.Utc
            ? command.Date
            : command.Date.ToUniversalTime();

        var transaction = Transaction.Create(
            amount,
            command.Name,
            utcDate,
            command.Type,
            userId
        );

        transaction.AssignCategory(category.Id);
        transaction.AssignPaymentMethod(paymentMethod.Id);

        if (!string.IsNullOrWhiteSpace(command.Description))
        {
            transaction.UpdateDetails(transaction.Name, command.Description);
        }

        if (command.IsCreditCardPurchase && command.Installment != null)
        {
            transaction.GenerateInstallments(command.Installment.Number);
        }

        if (command.IsReimbursement && command.Reimbursement != null)
        {
            var rAmount = Money.Create(command.Reimbursement.Amount);
            transaction.AddReimbursement(rAmount, command.Reimbursement.Reason ?? "No reason provided");
        }

        await _transactionRepository.AddTransactionAsync(transaction);

        var response = _mapper.Map<TransactionResponse>(transaction);

        return Result<TransactionResponse>.Success(response);
    }
}
