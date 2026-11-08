using FluentValidation;

namespace FinanceTracker.Application.Features.Receipts.Commands.UploadReceipt;

public sealed class UploadReceiptCommandValidator : AbstractValidator<UploadReceiptCommand>
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public UploadReceiptCommandValidator()
    {
        RuleFor(command => command.FileStream)
            .NotNull()
            .Must(stream => stream.Length > 0)
            .WithMessage("No file uploaded.");

        RuleFor(command => command.FileLength)
            .InclusiveBetween(1, MaxFileSizeBytes)
            .WithMessage("File must be between 1 byte and 5 MB.");

        RuleFor(command => command.OriginalFileName)
            .NotEmpty()
            .Must((_, fileName) => IsAllowedExtension(fileName))
            .WithMessage("Invalid file type. Only JPG, PNG, and PDF files are supported.");

        RuleFor(command => command.ContentType)
            .NotEmpty()
            .Must((command, contentType) => IsAllowedContentType(command.OriginalFileName, contentType))
            .WithMessage("Invalid file content type.");

        RuleFor(command => command.UserId)
            .NotEmpty();

        RuleFor(command => command.DisplayName)
            .NotEmpty()
            .MaximumLength(120);
    }

    private static bool IsAllowedExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAllowedContentType(string fileName, string contentType)
    {
        var expectedType = Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            _ => string.Empty
        };

        return contentType.Equals(expectedType, StringComparison.OrdinalIgnoreCase);
    }
}
