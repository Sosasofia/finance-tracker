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
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> Create([FromBody] TransactionCreateDTO transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Transaction cannot be null");
            }

            if (!UserId(out var userGuid))
            {
                return Unauthorized("Missing or invalid user ID claim");
            }

            var result = await _transactionService.AddTransactionAsync(transaction, userGuid);

            return result.Success ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetTransactionsByUser()
        {
            if (!UserId(out var userGuid))
            {
                return Unauthorized("Missing or invalid user ID claim");
            }

            var transactions = await _transactionService.GetTransactionsByUserAsync(userGuid);

            if (!transactions.Any())
            {
                return NotFound("No transactions found for this user");
            }

            return Ok(transactions);
        }

        private bool UserId(out Guid userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && Guid.TryParse(claim.Value, out userId))
            {
                return true;
            }
            
            userId = Guid.Empty;

            return false;
        }
    }
}
