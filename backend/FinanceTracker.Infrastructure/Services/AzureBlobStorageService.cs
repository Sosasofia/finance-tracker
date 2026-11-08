using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinanceTracker.Infrastructure.Services;

public class AzureBlobStorageService : IReceiptStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageSettings _settings;
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageSettings> options,
        ILogger<AzureBlobStorageService> logger)
    {
        _logger = logger;
        _blobServiceClient = blobServiceClient;
        _settings = options.Value;
        _containerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
    }

    public async Task<ReceiptStorageResult> UploadAsync(
        Stream fileStream,
        string originalFileName,
        string contentType,
        Guid userId,
        CancellationToken ct = default)
    {
        var receiptId = Guid.NewGuid().ToString("N");
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        var blobName = $"users/{userId}/receipts/{receiptId}{extension}";
        var blobClient = _containerClient.GetBlobClient(blobName);
        var uploadedAt = DateTimeOffset.UtcNow;

        try
        {
            await _containerClient.CreateIfNotExistsAsync(cancellationToken: ct);
            await blobClient.UploadAsync(
                fileStream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
                    Metadata = new Dictionary<string, string>
                    {
                        ["userId"] = userId.ToString(),
                        ["receiptId"] = receiptId,
                        ["status"] = "orphan",
                        ["uploadedAt"] = uploadedAt.ToString("O")
                    }
                },
                ct);

            var properties = await blobClient.GetPropertiesAsync(cancellationToken: ct);
            return new ReceiptStorageResult(receiptId, contentType, properties.Value.ContentLength, uploadedAt);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Receipt blob upload failed for user {UserId}.", userId);
            throw new InvalidOperationException("Receipt storage is unavailable.", ex);
        }
    }

    public async Task<ReceiptDownloadResult> OpenReadAsync(
        string receiptId,
        Guid userId,
        CancellationToken ct = default)
    {
        try
        {
            var blobClient = await FindBlobAsync(receiptId, userId, ct);
            var response = await blobClient.DownloadStreamingAsync(cancellationToken: ct);
            return new ReceiptDownloadResult(
                response.Value.Content,
                response.Value.Details.ContentType ?? "application/octet-stream",
                Path.GetFileName(blobClient.Name),
                response.Value.Details.ContentLength);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            throw new FileNotFoundException("Receipt was not found.", ex);
        }
        catch (FileNotFoundException)
        {
            throw new NotFoundException("Receipt", receiptId);
        }
    }

    public async Task<ReceiptSecureUrlResult> GetSecureUrlAsync(
        string receiptId,
        Guid userId,
        CancellationToken ct = default)
    {
        var blobClient = await FindBlobAsync(receiptId, userId, ct);
        var startsOn = DateTimeOffset.UtcNow.AddMinutes(-1);
        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(_settings.SasLifetimeMinutes);

        try
        {
            Uri sasUri;
            if (!string.IsNullOrWhiteSpace(_settings.ConnectionString))
            {
                sasUri = blobClient.GenerateSasUri(
                    BlobSasPermissions.Read,
                    expiresOn);
            }
            else
            {
                var delegationKey = await _blobServiceClient.GetUserDelegationKeyAsync(
                    startsOn,
                    expiresOn,
                    ct);
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _containerClient.Name,
                    BlobName = blobClient.Name,
                    Resource = "b",
                    StartsOn = startsOn,
                    ExpiresOn = expiresOn
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasParameters = sasBuilder.ToSasQueryParameters(
                    delegationKey.Value,
                    _blobServiceClient.AccountName);
                var uriBuilder = new BlobUriBuilder(blobClient.Uri)
                {
                    Sas = sasParameters
                };
                sasUri = uriBuilder.ToUri();
            }

            return new ReceiptSecureUrlResult(sasUri.ToString(), expiresOn);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Receipt SAS generation failed for user {UserId}.", userId);
            throw new InvalidOperationException("Receipt storage is unavailable.", ex);
        }
    }

    public async Task DeleteAsync(string receiptId, Guid userId, CancellationToken ct = default)
    {
        var blobClient = await FindBlobAsync(receiptId, userId, ct);
        try
        {
            await blobClient.DeleteIfExistsAsync(
                DeleteSnapshotsOption.IncludeSnapshots,
                cancellationToken: ct);
        }
        catch (FileNotFoundException)
        {
            throw new NotFoundException("Receipt", receiptId);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Receipt blob deletion failed for user {UserId}.", userId);
            throw new InvalidOperationException("Receipt storage is unavailable.", ex);
        }
    }

    private async Task<BlobClient> FindBlobAsync(string receiptId, Guid userId, CancellationToken ct)
    {
        if (!Guid.TryParse(receiptId, out var parsedReceiptId))
        {
            throw new FileNotFoundException("Receipt was not found.");
        }

        var normalizedReceiptId = parsedReceiptId.ToString("N");
        var prefix = $"users/{userId}/receipts/{normalizedReceiptId}";
        await foreach (var item in _containerClient.GetBlobsAsync(
            traits: BlobTraits.Metadata,
            states: BlobStates.None,
            prefix: prefix,
            cancellationToken: ct))
        {
            if (item.Metadata.TryGetValue("userId", out var owner) &&
                owner == userId.ToString() &&
                item.Metadata.TryGetValue("receiptId", out var storedId) &&
                string.Equals(storedId, normalizedReceiptId, StringComparison.OrdinalIgnoreCase))
            {
                return _containerClient.GetBlobClient(item.Name);
            }
        }

        throw new FileNotFoundException("Receipt was not found.");
    }
}
