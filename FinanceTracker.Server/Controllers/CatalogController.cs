using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository _catalogRepository;

        public CatalogController(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        [HttpGet("category")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _catalogRepository.GetCategories();

            return Ok(categories);
        }

        [HttpGet("payment-method")]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetPaymentMethods()
        {
            var paymentMethods = await _catalogRepository.GetPaymentMethods();

            return Ok(paymentMethods);
        }
    }
}
