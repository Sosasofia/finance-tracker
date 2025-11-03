using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.PaymentMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/payment-methods")]
public class PaymentMethodController : ControllerBase
{
    private readonly IPaymentMethodService _paymentMethodService;
    private readonly ICurrentUserService _currentUserService;

    public PaymentMethodController(IPaymentMethodService paymentMethodService, ICurrentUserService currentUserService)
    {
        _paymentMethodService = paymentMethodService;
        _currentUserService = currentUserService;
    }

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
