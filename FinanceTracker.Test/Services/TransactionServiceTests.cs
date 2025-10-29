using AutoMapper;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Transactions;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FinanceTracker.Test.Services
{
    public class TransactionServiceTests
    {
        [Fact]
        public async Task AddTransactionAsync_ReturnsSuccessResponse_WhenTransactionIsValid()
        {
            // Arrange
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockInstallmentService = new Mock<IInstallmentService>();
            var mockFileGenerator = new Mock<IFileGenerator>();
            var mockValidator = new Mock<IValidator<CreateTransactionDto>>();
            var mockMapper = new Mock<IMapper>();

            var userId = Guid.NewGuid();
            var transactionId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var paymentMethodId = Guid.NewGuid();

            var transactionCreateDto = new CreateTransactionDto
            {
                Amount = 150,
                Name = "Groceries",
                Description = "Supermarket purchase",
                Date = DateTime.UtcNow,
                Type = TransactionType.Expense,
                CategoryId = categoryId,
                PaymentMethodId = paymentMethodId,
                IsCreditCardPurchase = false,
                IsReimbursement = false
            };

            var transaction = new Transaction
            {
                Id = transactionId,
                Amount = transactionCreateDto.Amount,
                Name = transactionCreateDto.Name,
                Description = transactionCreateDto.Description,
                Date = transactionCreateDto.Date,
                Type = transactionCreateDto.Type,
                CategoryId = transactionCreateDto.CategoryId,
                PaymentMethodId = transactionCreateDto.PaymentMethodId,
                UserId = userId
            };

            mockTransactionRepository
                .Setup(repo => repo.AddTransactionAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(transaction);

            var transactionResponse = new TransactionResponse
            {
                Id = transactionId,
                Amount = transactionCreateDto.Amount,
                Name = transactionCreateDto.Name,
                Description = transactionCreateDto.Description,
                Date = transactionCreateDto.Date,
                Type = transactionCreateDto.Type,
                CategoryId = transactionCreateDto.CategoryId.Value,
                PaymentMethodId = transactionCreateDto.PaymentMethodId.Value
            };

            // Validator returns success
            mockValidator
                .Setup(v => v.ValidateAsync(transactionCreateDto, default))
                .ReturnsAsync(new ValidationResult());

            // Mapper maps CreateTransactionDto to Transaction
            mockMapper
                .Setup(m => m.Map<Transaction>(transactionCreateDto))
                .Returns(transaction);

            // Repository returns the transaction
            mockTransactionRepository
                .Setup(repo => repo.AddTransactionAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(transaction);

            // Mapper maps Transaction to TransactionResponse
            mockMapper
                .Setup(m => m.Map<TransactionResponse>(transaction))
                .Returns(transactionResponse);

            var transactionService = new TransactionService(
                mockTransactionRepository.Object,
                mockInstallmentService.Object,
                mockFileGenerator.Object,
                mockValidator.Object,
                mockMapper.Object
            );

            // Act
            var result = await transactionService.AddTransactionAsync(transactionCreateDto, userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(transactionId, result.Value.Id);
            Assert.Equal("Groceries", result.Value.Name);
        }
    }
}
