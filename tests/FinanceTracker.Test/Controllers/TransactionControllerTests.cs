using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Features.Transactions.Commands.CreateTransaction;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Server.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceTracker.Test.Controllers;

public class TransactionControllerTests
{
    private readonly Mock<ISender> _mediatorMock;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly TransactionController _transactionController;
    private readonly Guid _userId;
    private readonly Guid _categoryId;
    private readonly Guid _paymentMethodId;

    public TransactionControllerTests()
    {
        _mediatorMock = new Mock<ISender>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();

        _transactionController = new TransactionController(_mockCurrentUserService.Object, _mediatorMock.Object);
        _userId = Guid.NewGuid();
        _categoryId = Guid.NewGuid();
        _paymentMethodId = Guid.NewGuid();
    }

    [Fact]
    public async Task CreateTransaction_ReturnsCreatedAtAction_WhenCommandIsValid()
    {
        // Arrange
        _mockCurrentUserService.Setup(s => s.UserId()).Returns(_userId);

        var command = new CreateTransactionCommand(
            100m,
            "Test transaction name",
            DateTime.UtcNow,
            TransactionType.Expense,
            _categoryId,
            _paymentMethodId
        );

        var expectedResponse = new TransactionResponse
        {
            Id = Guid.NewGuid(),
            Amount = 100m,
            Name = "Test transaction name",
            Description = "Test Transaction",
            CategoryId = _categoryId,
            Type = TransactionType.Expense,
            Date = command.Date
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTransactionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _transactionController.Create(command, default);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<TransactionResponse>(createdResult.Value);

        Assert.Equal(100m, response.Amount);
        Assert.Equal("Test transaction name", response.Name);
        Assert.Equal("Test Transaction", response.Description);
    }

    [Fact]
    public async Task CreateTransaction_Throws_WhenUserIdClaimIsMissing()
    {
        // Arrange
        var command = new CreateTransactionCommand(
            100m,
            "Test transaction name",
            DateTime.UtcNow,
            TransactionType.Expense,
            _categoryId,
            _paymentMethodId
        );

        _mockCurrentUserService
            .Setup(s => s.UserId())
            .Throws(new InvalidOperationException("Missing or invalid user ID claim"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _transactionController.Create(command, default)
        );
        Assert.Equal("Missing or invalid user ID claim", ex.Message);
    }
}
