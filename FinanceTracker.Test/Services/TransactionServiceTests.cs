using AutoMapper;
using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Models.Response;
using FinanceTracker.Server.Repositories;
using FinanceTracker.Server.Services;
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

            var userId = Guid.NewGuid();
            var transactionId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var transactionCreateDto = new TransactionCreateDTO
            {
                Amount = 150,
                Name = "Groceries",
                Description = "Supermarket purchase",
                Date = DateTime.UtcNow,
                Type = TransactionType.Expense,
                CategoryId = categoryId,
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
                UserId = userId
            };

            mockTransactionRepository
                .Setup(repo => repo.AddTransactionAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(transaction);

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Transaction, TransactionResponse>();
                cfg.CreateMap<TransactionCreateDTO, Transaction>();
            });

            var mapper = configuration.CreateMapper();

            var service = new TransactionService(mockTransactionRepository.Object, mapper);

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
