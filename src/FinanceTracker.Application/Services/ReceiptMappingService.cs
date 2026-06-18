using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Common.Utilities;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Domain.ValueObjects;

namespace FinanceTracker.Application.Services;

public class ReceiptMappingService : IReceiptMappingService
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly IPaymentMethodRepository _paymentRepo;

    public ReceiptMappingService(
        ICategoryRepository categoryRepo,
        IPaymentMethodRepository paymentRepo)
    {
        _categoryRepo = categoryRepo;
        _paymentRepo = paymentRepo;
    }

    public async Task<ExtractedReceiptDto> MapAzureResultAsync(
        ExtractedReceiptData azureData,
        string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
        {
            throw new ArgumentException("Invalid user identifier format.", nameof(userId));
        }

        var categories = await _categoryRepo.GetCategories(userGuid, default);
        var paymentMethods = await _paymentRepo.GetPaymentMethods(userGuid, default);

        Guid? bestPaymentMethodId = null;
        double maxPaymentConfidence = 0.0;
        string? suggestedPaymentText = null;

        if (!string.IsNullOrWhiteSpace(azureData.RawContent))
        {
            var rawTextLower = azureData.RawContent.ToLowerInvariant();

            foreach (var method in paymentMethods)
            {
                var typeKeyword = method.Type.ToString().ToLowerInvariant();
                var nameKeyword = method.Name.ToLowerInvariant();

                if (rawTextLower.Contains(nameKeyword))
                {
                    bestPaymentMethodId = method.Id;
                    maxPaymentConfidence = 0.85;
                    suggestedPaymentText = method.Name;
                    break;
                }

                if (rawTextLower.Contains(typeKeyword))
                {
                    bestPaymentMethodId = method.Id;
                    maxPaymentConfidence = 0.80;
                    suggestedPaymentText = method.Type.ToString();
                    break;
                }
            }
        }

        Guid? bestCategoryId = null;
        double maxCategoryConfidence = 0.0;
        var categorySearchTarget = azureData.ReceiptType;

        if (!string.IsNullOrWhiteSpace(categorySearchTarget))
        {
            foreach (var category in categories)
            {
                var score = StringMatcher.CalculateSimilarity(categorySearchTarget, category.Name);
                if (score > maxCategoryConfidence)
                {
                    maxCategoryConfidence = score;
                    bestCategoryId = category.Id;
                }
            }
        }


        var finalPaymentId = maxPaymentConfidence >= 0.70 ? bestPaymentMethodId : null;
        var finalCategoryId = maxCategoryConfidence >= 0.70 ? bestCategoryId : null;

        return new ExtractedReceiptDto(
            TransactionDate: azureData.TransactionDate?.ToString(),
            Amount: (decimal?)azureData.TotalAmount,
            MerchantName: azureData.MerchantName,
            CategoryId: finalCategoryId,
            CategoryConfidence: maxCategoryConfidence > 0 ? Math.Round(maxCategoryConfidence, 2) : null,
            PaymentMethodId: finalPaymentId,
            PaymentMethodConfidence: maxPaymentConfidence > 0 ? Math.Round(maxPaymentConfidence, 2) : null,
            RawCategoryText: categorySearchTarget,
            RawPaymentMethodText: suggestedPaymentText,
            PaymentType: null,
            RawContent: azureData.RawContent,
            LineItems: azureData.LineItems != null ? azureData.LineItems.ToArray() : Array.Empty<string>()
        );
    }
}
