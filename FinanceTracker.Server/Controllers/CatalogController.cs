using AutoMapper;
using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CatalogController : BaseController
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly IMapper _mapper;

        public CatalogController(ICatalogRepository catalogRepository, IMapper mapper)
        {
            _catalogRepository = catalogRepository;
            _mapper = mapper;
        }

        [HttpGet("category")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                var categories = await _catalogRepository.GetCategories();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("custom-category")]
        [Authorize(AuthenticationSchemes = "CustomJWT")]
        public async Task<ActionResult<IEnumerable<CustomCategory>>> GetCustomCategories()
        {
            if (!UserId(out var userGuid))
            {
                return Unauthorized("Missing or invalid user ID claim");
            }
            try
            {
                var customCategories = await _catalogRepository.GetCustomCategoriesAsync(userGuid);
                return Ok(customCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("custom-category")]
        [Authorize(AuthenticationSchemes = "CustomJWT")]
        public async Task<ActionResult<CustomCategory>> AddCustomCategory([FromBody] CustomCategoryDTO categoryDTO)
        {
            if (!UserId(out var userGuid))
            {
                return Unauthorized("Missing or invalid user ID claim");
            }
            if (categoryDTO == null || string.IsNullOrWhiteSpace(categoryDTO.Name))
            {
                return BadRequest("Invalid custom category data.");
            }

            // Check if the category already exists for the user
            var existingCategory = await _catalogRepository.GetCustomCategoriesAsync(userGuid);
            if (existingCategory.Any(c => c.Name.Equals(categoryDTO.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("A custom category with this name already exists for this user.");
            }

            var newCategory = _mapper.Map<CustomCategory>(categoryDTO);
            newCategory.UserId = userGuid;

            try
            {
                var createdCategory = await _catalogRepository.AddCustomCategoryAsync(newCategory);
                return CreatedAtAction(nameof(GetCustomCategories), new { id = createdCategory.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("payment-method")]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetPaymentMethods()
        {
            try
            {
                var paymentMethods = await _catalogRepository.GetPaymentMethods();

                return Ok(paymentMethods);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
