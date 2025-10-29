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

            var expectedResponse = new TransactionResponse
            {
                Id = Guid.NewGuid(),
                Amount = transactionCreateDTO.Amount,
                Description = transactionCreateDTO.Description,
                PaymentMethodId = transactionCreateDTO.PaymentMethodId.Value,
                CategoryId = transactionCreateDTO.CategoryId.Value,
                Type = transactionCreateDTO.Type,
                Date = transactionCreateDTO.Date
            };

            _mockCurrentUserService.Setup(s => s.UserId()).Returns(_userId);

            _mockCurrentUserService.Setup(s => s.UserId()).Returns(_userId);
            _mockTransactionService
                .Setup(service => service.AddTransactionAsync(transactionCreateDTO, _userId))
                .ReturnsAsync(Result<TransactionResponse>.Success(expectedResponse));


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
            Assert.Equal("The transaction object cannot be null.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_Throws_WhenUserIdClaimIsMissing()
        {
            // Arrange
            var transactionCreateDTO = CreateTestTransactionDto();
            _mockCurrentUserService
                .Setup(s => s.UserId())
                .Throws(new InvalidOperationException("Missing or invalid user ID claim"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _transactionController.Create(transactionCreateDTO));
            Assert.Equal("Missing or invalid user ID claim", ex.Message);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            var expectedErrors = new List<string> { "Error during transaction creation." };
            _mockCurrentUserService.Setup(s => s.UserId()).Returns(_userId);
            _mockTransactionService
                .Setup(service => service.AddTransactionAsync(It.IsAny<CreateTransactionDto>(), It.IsAny<Guid>()))
                .ReturnsAsync(Result<TransactionResponse>.Failure(expectedErrors));

            // Act
            var actionResult = await _transactionController.Create(CreateTestTransactionDto());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);

            var returnedValue = badRequestResult.Value;
            Assert.NotNull(returnedValue);

            var errorsProperty = returnedValue.GetType().GetProperty("errors");
            Assert.NotNull(errorsProperty);

            var actualErrors = errorsProperty.GetValue(returnedValue) as List<string>;
            Assert.NotNull(actualErrors);

            Assert.Equal(expectedErrors, actualErrors);
        }

        private CreateTransactionDto CreateTestTransactionDto()
        {
            return new CreateTransactionDto
            {
                Amount = 100,
                Name = "Test transaction name",
                Description = "Test Transaction",
                CategoryId = Guid.NewGuid(),
                PaymentMethodId = Guid.NewGuid(),
                Type = TransactionType.Expense,
                Date = DateTime.UtcNow,
                IsCreditCardPurchase = false,
                IsReimbursement = false
            };
        }
    }
}
