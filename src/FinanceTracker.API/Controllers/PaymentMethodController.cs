using AutoMapper;

using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/payment-method")]
public class PaymentMethodController : BaseController
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public PaymentMethodController(IPaymentMethodRepository paymentMethodRepository, IMapper mapper)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetPaymentMethods()
    {
        try
        {
            var paymentMethods = await _paymentMethodRepository.GetPaymentMethods();

            return Ok(paymentMethods);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}