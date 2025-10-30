using AutoMapper;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.PaymentMethods;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Route("api/payment-methods")]
public class PaymentMethodController : ControllerBase
{
    private readonly IPaymentMethodService _paymentMethodService;

    public PaymentMethodController(IPaymentMethodService paymentMethodService, IMapper mapper)
    {
        _paymentMethodService = paymentMethodService;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetPaymentMethods()
    {
        var paymentMethods = await _paymentMethodService.GetPaymentMethodsAsync();

        if (paymentMethods == null)
        {
            return NotFound();
        }

        return Ok(paymentMethods);
    }
}
