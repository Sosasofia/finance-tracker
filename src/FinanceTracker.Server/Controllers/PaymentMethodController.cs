using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.PaymentMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/payment-methods")]
[Produces("application/json")]
[Consumes("application/json")]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
public class PaymentMethodController : ControllerBase
{
    private readonly IPaymentMethodService _paymentMethodService;
    private readonly ICurrentUserService _currentUserService;

    public PaymentMethodController(IPaymentMethodService paymentMethodService, ICurrentUserService currentUserService)
    {
        _paymentMethodService = paymentMethodService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Retrieves all payment methods available to the authenticated user.
    /// </summary>
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Returns the list of payment methods available to the user performing the request.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>Includes all payment methods created by the authenticated user.</li>
    ///   <li>Examples: <em>bank accounts, debit cards, credit cards, virtual wallets, etc.</em></li>
    ///   <li>Returns an empty list if the user has no payment methods.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>200 OK</c> with the user’s payment methods.</p>
    /// </remarks>
    ///
    /// <response code="200">A list of user-owned payment methods.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PaymentMethodDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetPaymentMethods()
    {
        var paymentMethods = await _paymentMethodService.GetPaymentMethodsAsync();

        return Ok(paymentMethods);
    }

    /// <summary>
    /// Retrieves a specific payment method by its unique identifier.
    /// </summary>
    ///
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Looks up a payment method that matches the provided ID.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>The payment method must belong to the authenticated user.</li>
    ///   <li>If the item does not exist, a <c>404 Not Found</c> response is returned.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>200 OK</c> with the matched payment method.</p>
    /// </remarks>
    ///
    /// <param name="id">The unique identifier of the payment method.</param>
    /// <response code="200">Returns the payment method.</response>
    /// <response code="404">If no payment method with the given id exists.</response>
    [HttpGet("{id}", Name = "GetPaymentMethodById")]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentMethodDto>> GetPaymentMethodById(Guid id)
    {
        var paymentMethod = await _paymentMethodService.GetByIdAsync(id);

        return Ok(paymentMethod);
    }

    /// <summary>
    /// Creates a new payment method for the authenticated user.
    /// </summary>
    ///
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Adds a payment method and links it to the user performing the request.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>Validates the provided data.</li>
    ///   <li>Associates the new record with the authenticated user.</li>
    ///   <li>Returns a <strong>Location</strong> header pointing to the new resource.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>201 Created</c> along with the newly created payment method.</p>
    /// </remarks>
    ///
    /// <param name="dto">The details used to create the payment method.</param>
    /// <response code="201">Returns the created payment method and a URL to retrieve it via the Location header.</response>
    /// <response code="400">Returned when the request fails validation.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentMethodDto>> AddPaymentMethod([FromBody] CreatePaymentMethodDto dto)
    {
        var userId = _currentUserService.UserId();

        var created = await _paymentMethodService.CreateAsync(userId, dto);

        return CreatedAtAction(
            nameof(GetPaymentMethodById),
            new { id = created.Id },
            created
        );
    }

    /// <summary>
    /// Deletes an existing payment method owned by the authenticated user.
    /// </summary>
    ///
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Removes a payment method using its unique identifier.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>The payment method must belong to the authenticated user.</li>
    ///   <li>If it does not exist, a <c>404 Not Found</c> response is returned.</li>
    ///   <li>Successful deletion produces no response body.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>204 No Content</c> on successful deletion.</p>
    /// </remarks>
    ///
    /// <param name="id">The ID of the payment method to delete.</param>
    /// <response code="204">Payment method deleted successfully.</response>
    /// <response code="404">If the payment method does not exist.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePaymentMethod(Guid id)
    {
        await _paymentMethodService.DeleteAsync(id);

        return NoContent();
    }
}
