using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Features.PaymentMethods.Commands.CreatePaymentMethod;
using FinanceTracker.Application.Features.PaymentMethods.Commands.DeletePaymentMethod;
using FinanceTracker.Application.Features.PaymentMethods.Models;
using FinanceTracker.Application.Features.PaymentMethods.Queries.GetById;
using FinanceTracker.Application.Features.PaymentMethods.Queries.GetPaymentMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Authorize]
[EnableRateLimiting("fixed")]
[Route("api/payment-methods")]
[Produces("application/json")]
[Consumes("application/json")]
public class PaymentMethodController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly GetPaymentMethodsQueryHandler _getListHandler;
    private readonly GetPaymentMethodByIdQueryHandler _getByIdHandler;
    private readonly CreatePaymentMethodCommandHandler _createHandler;
    private readonly DeletePaymentMethodCommandHandler _deleteHandler;

    public PaymentMethodController(
        ICurrentUserService currentUserService,
        GetPaymentMethodsQueryHandler getListHandler,
        GetPaymentMethodByIdQueryHandler getByIdHandler,
        CreatePaymentMethodCommandHandler createHandler,
        DeletePaymentMethodCommandHandler deleteHandler)
    {
        _currentUserService = currentUserService;
        _getListHandler = getListHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _deleteHandler = deleteHandler;
    }

    /// <summary>
    /// Retrieves a collection of available payment methods.
    /// </summary>
    /// <returns>An <see cref="ActionResult{T}"/> containing a collection of <see cref="PaymentMethodDto"/> objects representing
    /// the available payment methods. Returns an empty collection if no payment methods are available.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetAll()
    {
        var query = new GetPaymentMethodsQuery(_currentUserService.UserId());
        var result = await _getListHandler.Handle(query, default);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the payment method with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the payment method to retrieve.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing the payment method data if found; otherwise, a 404 Not Found
    /// response.</returns>
    [HttpGet("{id:guid}", Name = "GetById")]
    public async Task<ActionResult<PaymentMethodDto>> GetById([FromRoute] Guid id)
    {
        var query = new GetPaymentMethodByIdQuery(id, _currentUserService.UserId());
        var result = await _getByIdHandler.Handle(query, default);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new payment method for the current user.
    /// </summary>
    /// <param name="command">The command containing the details of the payment method to create. Must not be null.</param>
    /// <returns>An ActionResult containing the created payment method details. Returns a 201 Created response with the new
    /// payment method if successful.</returns>
    [HttpPost]
    public async Task<ActionResult<PaymentMethodDto>> AddPaymentMethod([FromBody] CreatePaymentMethodCommand command)
    {
        var commandWithUser = command with { UserId = _currentUserService.UserId() };
        var created = await _createHandler.Handle(commandWithUser, default);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created
        );
    }

    /// <summary>
    /// Deletes the payment method with the specified unique identifier for the current user.
    /// </summary>
    /// <param name="id">The unique identifier of the payment method to delete.</param>
    /// <returns>An <see cref="NoContentResult"/> if the payment method is successfully deleted; otherwise, an appropriate error
    /// response.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePaymentMethod([FromRoute] Guid id)
    {
        var command = new DeletePaymentMethodCommand(id, _currentUserService.UserId());
        await _deleteHandler.Handle(command, default);

        return NoContent();
    }
}
