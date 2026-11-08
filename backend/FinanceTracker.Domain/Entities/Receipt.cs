namespace FinanceTracker.Domain.Entities;

public class Receipt
{
    private Receipt() { }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string StorageReceiptId { get; private set; }
    public string DisplayName { get; private set; }
    public string OriginalFileName { get; private set; }
    public string ContentType { get; private set; }
    public long Size { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public Guid? TransactionId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public static Receipt Create(
        Guid userId,
        string storageReceiptId,
        string displayName,
        string originalFileName,
        string contentType,
        long size)
    {
        return new Receipt
        {
            Id = Guid.Parse(storageReceiptId),
            UserId = userId,
            StorageReceiptId = storageReceiptId,
            DisplayName = displayName.Trim(),
            OriginalFileName = originalFileName,
            ContentType = contentType,
            Size = size,
            UploadedAt = DateTime.UtcNow
        };
    }

    public void Rename(string displayName)
    {
        DisplayName = displayName.Trim();
    }

    public void AttachToTransaction(Guid transactionId)
    {
        TransactionId = transactionId;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
