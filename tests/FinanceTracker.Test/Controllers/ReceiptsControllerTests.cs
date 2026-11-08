using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Features.Receipts.Queries.GetReceiptUrl;
using FinanceTracker.Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceTracker.Test.Controllers;

public class ReceiptsControllerTests
{
    [Fact]
    public async Task GetReceiptUrl_ReturnsUrlForAuthenticatedUser()
    {
        var dispatcher = new Mock<IRequestDispatcher>();
        var currentUser = new Mock<ICurrentUserService>();
        var userId = Guid.NewGuid();
        var receiptId = Guid.NewGuid().ToString("N");
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(15);
        var response = new ReceiptSecureUrlResponse(
            "https://storage.example/receipts/receipt.jpg?sv=...",
            expiresAt);

        currentUser.Setup(service => service.UserId()).Returns(userId);
        dispatcher
            .Setup(service => service.Send(
                It.Is<GetReceiptUrlQuery>(query =>
                    query.ReceiptId == receiptId && query.UserId == userId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var controller = new ReceiptsController(dispatcher.Object, currentUser.Object);

        var result = await controller.GetReceiptUrl(receiptId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
    }
}
