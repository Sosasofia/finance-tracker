using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Response;
using FinanceTracker.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTracker.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "CustomJWT, GoogleJWT")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> Post([FromBody] TransactionCreateDTO transaction)
        {
            if (transaction == null) return BadRequest("Transaction cannot be null");

            if (transaction.IsCreditCardPurchase && (transaction.Installment == null || transaction.Installment.Number <= 0))
            {
                return BadRequest("The number of installments must be greater than zero.");
            }

            var result = await _transactionService.AddTransactionAsync(transaction);

            return result.Success ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetTransactionsByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("Missing user ID");
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Invalid user ID");
            }

            var transactions = await _transactionService.GetTransactionsByUserAsync(userGuid);

            if (!transactions.Any())
            {
                return NotFound("No transactions found for this user");
            }

            return Ok(transactions);
        }
    }
}
