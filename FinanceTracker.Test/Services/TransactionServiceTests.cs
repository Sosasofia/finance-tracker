using AutoMapper;
using FinanceTracker.Application.Features.Transactions;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Repositories;
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
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var mockPaymentMethodRepository = new Mock<IPaymentMethodRepository>();

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

            // Setup the catalog repository to return true for category and payment method existence
            mockCategoryRepository.Setup(repo => repo.CategoryExistsAsync(categoryId)).ReturnsAsync(true);
            mockPaymentMethodRepository.Setup(repo => repo.PaymentMethodExistsAsync(paymentMethodId)).ReturnsAsync(true);

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Transaction, TransactionResponse>();
                cfg.CreateMap<CreateTransactionDto, Transaction>()
                    .ForMember(dest => dest.PaymentMethodId, opt => opt.MapFrom(src => src.PaymentMethodId)); // Map PaymentMethodId
            });

            var mapper = configuration.CreateMapper();

            var service = new TransactionService(mockTransactionRepository.Object, mapper, mockCategoryRepository.Object,mockPaymentMethodRepository.Object); // Pass the mock catalog repository

            // Act
            var result = await service.AddTransactionAsync(transactionCreateDto, userId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(transactionId, result.Data.Id);
            Assert.Equal("Groceries", result.Data.Name);
        }
    }
}
