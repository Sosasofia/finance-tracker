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
        [Authorize(AuthenticationSchemes = "CustomJWT, GoogleJWT")]
        public ActionResult<IEnumerable<TransactionResponse>> GetTransactionsByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var userGuid = Guid.Parse(userId);

            var transactions = _transactionService.GetTransactionsByUserAsync(userGuid).Result;

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
