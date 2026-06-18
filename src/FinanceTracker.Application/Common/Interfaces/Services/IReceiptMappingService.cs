using FinanceTracker.Application.DTOs;
using FinanceTracker.Domain.ValueObjects;

namespace FinanceTracker.Application.Common.Interfaces.Services;
public interface IReceiptMappingService
{
    /// <summary>
    /// Takes the raw text and extracted properties from an Azure Receipt scan,
    /// performs native semantic fuzzy-matching against the user's database records,
    /// and returns a structured DTO complete with matched IDs and confidence levels.
    /// </summary>
    /// <param name="azureData">The raw result object returned from Azure Document Intelligence.</param>
    /// <param name="userId">The unique identifier of the authenticated user making the request.</param>
    /// <returns>A data transfer object ready to populate the frontend transaction form.</returns>
    Task<ExtractedReceiptDto> MapAzureResultAsync(ExtractedReceiptData azureData, string userId);
}
