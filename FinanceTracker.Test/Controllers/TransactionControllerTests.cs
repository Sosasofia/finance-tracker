using FinanceTracker.Server.Controllers;
using FinanceTracker.Server.Enums;
using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.DTOs.Response;
using FinanceTracker.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FinanceTracker.Test.Controllers
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly TransactionController _transactionController;

        public TransactionControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _transactionController = new TransactionController(_mockTransactionService.Object);
        }


        [Fact]
        public async Task CreateTransaction_ReturnsOk_WhenTransactionIsValid()
        {
            // Arrange
            // Authenticate user
            var userId = Guid.NewGuid();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            _transactionController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var transactionCreateDTO = new TransactionCreateDTO
            {
                Amount = 100,
                Description = "Test Transaction",
                CategoryId = Guid.NewGuid(),
                Type = TransactionType.Expense,
                Date = DateTime.UtcNow,
                IsCreditCardPurchase = false,
                IsReimbursement = false
            };

            var expectedResponse = new Response<TransactionResponse>(new TransactionResponse
            {
                Id = Guid.NewGuid(),
                Amount = 100,
                Description = "Test Transaction",
                CategoryId = Guid.NewGuid(), // deberia fallar
                Type = TransactionType.Expense,
                Date = DateTime.UtcNow,
            });

            _mockTransactionService
                .Setup(static service => service.AddTransactionAsync(It.IsAny<TransactionCreateDTO>(), It.IsAny<Guid>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _transactionController.Create(transactionCreateDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<TransactionResponse>(okResult.Value);
            Assert.Equal(100, response.Amount);
            Assert.Equal("Test Transaction", response.Description);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsBadRequest_WhenTransactionIsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            _transactionController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

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
            // Usuario sin claim de ID
            var user = new ClaimsPrincipal(new ClaimsIdentity()); // identidad vacía

            _transactionController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var transactionCreateDTO = new TransactionCreateDTO
            {
                Amount = 100,
                Description = "Test Transaction",
                CategoryId = Guid.NewGuid(),
                Type = TransactionType.Expense,
                Date = DateTime.UtcNow,
                IsCreditCardPurchase = false,
                IsReimbursement = false
            };

            // Act
            var result = await _transactionController.Create(transactionCreateDTO);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Missing or invalid user ID claim", unauthorizedResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsBadRequest_WhenServiceReturnsFailure()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            _transactionController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var transactionCreateDTO = new TransactionCreateDTO
            {
                Amount = 100,
                Description = "Test Transaction",
                CategoryId = Guid.NewGuid(),
                Type = TransactionType.Expense,
                Date = DateTime.UtcNow,
                IsCreditCardPurchase = false,
                IsReimbursement = false
            };

            var expectedResponse = new Response<TransactionResponse>("Error creating transaction");

            _mockTransactionService
                .Setup(service => service.AddTransactionAsync(It.IsAny<TransactionCreateDTO>(), It.IsAny<Guid>()))
                .ReturnsAsync(expectedResponse);

            // Act

            var result = await _transactionController.Create(transactionCreateDTO);
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var responseMessage = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal("Error creating transaction", responseMessage);
        }
    }
}
