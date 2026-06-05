using Azure;
using Azure.AI.DocumentIntelligence;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Domain.ValueObjects;


namespace FinanceTracker.Infrastructure.Services;

public class AzureReceiptScannerService : IReceiptScannerService
{
    private readonly DocumentIntelligenceClient _client;

    public AzureReceiptScannerService(string endpoint, string apiKey)
    {
        _client = new DocumentIntelligenceClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
    }

    public async Task<ExtractedReceiptData> ScanReceiptAsync(Stream receiptStream)
    {
        var binaryData = BinaryData.FromStream(receiptStream);

        var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-receipt", binaryData);
        var result = operation.Value;

        var document = result.Documents.FirstOrDefault();
        if (document == null) return new ExtractedReceiptData();

        var extractedData = new ExtractedReceiptData();

        if (document.Fields.TryGetValue("MerchantName", out var merchantField))
        {
            extractedData.MerchantName = merchantField.ValueString;
        }

        if (document.Fields.TryGetValue("Total", out var totalField))
        {
            extractedData.TotalAmount = totalField.ValueCurrency?.Amount;
        }

        if (document.Fields.TryGetValue("TransactionDate", out var dateField))
        {
            extractedData.TransactionDate = dateField.ValueDate?.ToString("yyyy-MM-dd");
        }

        if (document.Fields.TryGetValue("Items", out var itemsField) && itemsField.FieldType == DocumentFieldType.List)
        {
            foreach (var item in itemsField.ValueList)
            {
                if (item.FieldType == DocumentFieldType.Dictionary)
                {
                    if (item.ValueDictionary.TryGetValue("Description", out var descriptionField))
                    {
                        var itemName = descriptionField.ValueString;
                        if (!string.IsNullOrWhiteSpace(itemName))
                        {
                            extractedData.LineItems.Add(itemName);
                        }
                    }
                }
            }
        }

        return extractedData;
    }
}
