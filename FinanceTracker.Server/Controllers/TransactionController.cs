using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Response;
using FinanceTracker.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public ActionResult<TransactionResponse> Post([FromBody] TransactionCreateDTO transaction)
        {
            if (transaction == null) return BadRequest("Transaction cannot be null");

            var result = _transactionService.AddTransactionAsync(transaction).Result;

            if (result == null)
            {
                return StatusCode(500, "Internal server error");
            }

            return result.Success ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet]
        public ActionResult<IEnumerable<TransactionResponse>> GetTransactionsByUser(Guid userId)
        {
            var transactions = _transactionService.GetTransactionsByUserAsync(userId).Result;

            if (transactions == null)
            {
                return StatusCode(500, "Internal server error");
            }
            if (!transactions.Any())
            {
                return NotFound("No transactions found for this user");
            }
            return Ok(transactions);
        }
    }
}
