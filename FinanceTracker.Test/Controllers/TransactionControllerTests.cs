using FinanceTracker.Application.Common.DTOs;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Transactions;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceTracker.Test.Controllers
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly TransactionController _transactionController;

        private readonly Guid _userId;

        public TransactionControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _transactionController = new TransactionController(_mockTransactionService.Object, _mockCurrentUserService.Object);
            _userId = Guid.NewGuid();
        }

        [Fact]
        public async Task CreateTransaction_ReturnsOk_WhenTransactionIsValid()
        {
            // Arrange
            var transactionCreateDTO = CreateTestTransactionDto();

            var expectedResponse = new Response<TransactionResponse>(new TransactionResponse
            {
                Id = Guid.NewGuid(),
                Amount = transactionCreateDTO.Amount,
                Description = transactionCreateDTO.Description,
                CategoryId = transactionCreateDTO.CategoryId, 
                Type = transactionCreateDTO.Type,
                Date = transactionCreateDTO.Date,
            });

            _mockCurrentUserService.Setup(s => s.UserId()).Returns(_userId);

            _mockTransactionService
                .Setup(static service => service.AddTransactionAsync(It.IsAny<CreateTransactionDto>(), It.IsAny<Guid>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _transactionController.Create(transactionCreateDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<TransactionResponse>(okResult.Value);
            Assert.Equal(transactionCreateDTO.Amount, response.Amount);
            Assert.Equal(transactionCreateDTO.Description, response.Description);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsBadRequest_WhenTransactionIsNull()
        {
            // Arrange
            _mockCurrentUserService.Setup(s => s.UserId()).Returns(Guid.NewGuid());

            // Act
            var result = await _transactionController.Create(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Transaction cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsUnauthorized_WhenUserIdClaimIsMissing()
        {
            // Arrange
            var transactionCreateDTO = CreateTestTransactionDto();

            // Act
            var result = await _transactionController.Create(transactionCreateDTO);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Missing or invalid user ID claim", unauthorizedResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            var expectedResponse = new Response<TransactionResponse>("Error creating transaction");

            _mockCurrentUserService.Setup(s => s.UserId()).Returns(_userId);

            _mockTransactionService
                .Setup(service => service.AddTransactionAsync(It.IsAny<CreateTransactionDto>(), It.IsAny<Guid>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _transactionController.Create(new CreateTransactionDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var responseMessage = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal(expectedResponse.Message, responseMessage);
        }

        private CreateTransactionDto CreateTestTransactionDto()
        {
            return new CreateTransactionDto
            {
                Amount = 100,
                Description = "Test Transaction",
                CategoryId = Guid.NewGuid(),
                Type = TransactionType.Expense,
                Date = DateTime.UtcNow,
                IsCreditCardPurchase = false,
                IsReimbursement = false
            };
        }
    }
}
