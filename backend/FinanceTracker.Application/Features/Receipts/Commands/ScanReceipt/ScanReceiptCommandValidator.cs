using FluentValidation;

namespace FinanceTracker.Application.Features.Receipts.Commands.ScanReceipt;

public sealed class ScanReceiptCommandValidator : AbstractValidator<ScanReceiptCommand>
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public ScanReceiptCommandValidator()
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
            .Must(fileName => new[] { ".jpg", ".jpeg", ".png", ".pdf" }
                .Contains(Path.GetExtension(fileName).ToLowerInvariant()))
            .WithMessage("Invalid file type. Only JPG, PNG, and PDF files are supported.");

        RuleFor(command => command.UserId)
            .NotEmpty();
    }
}
