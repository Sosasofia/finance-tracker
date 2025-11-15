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
[ApiConventionType(typeof(DefaultApiConventions))]
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
    /// You can search for Accounts here.
    /// </summary>

    /// <remarks>
    /// All the parameters in the request body can be null. 
    ///
    ///  You can search by using any of the parameters in the request.
    ///  
    ///  NOTE: You can only search by one parameter at a time
    ///  
    /// Sample request:
    ///
    ///     POST /Account
    ///     {
    ///        "userId": null,
    ///        "bankId": null,
    ///        "dateCreated": null
    ///     }
    ///     OR
    ///     
    ///     POST /Account
    ///     {
    ///        "userId": null,
    ///        "bankId": 000,
    ///        "dateCreated": null
    ///     } 
    /// </remarks>
    /// <param name="request"></param>
    /// <returns> This endpoint returns a list of Accounts.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetPaymentMethods()
    {
        var paymentMethods = await _paymentMethodService.GetPaymentMethodsAsync();

        return Ok(paymentMethods);
    }

    [HttpGet("{id}", Name = "GetPaymentMethodById")]
    public async Task<ActionResult<PaymentMethodDto>> GetPaymentMethodById(Guid id)
    {
        var paymentMethod = await _paymentMethodService.GetByIdAsync(id);

        return Ok(paymentMethod);
    }

    [HttpPost]
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaymentMethod(Guid id)
    {
        await _paymentMethodService.DeleteAsync(id);

        return NoContent();
    }
}
