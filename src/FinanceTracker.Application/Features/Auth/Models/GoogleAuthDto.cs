namespace FinanceTracker.Application.Features.Auth.Models;

public class GoogleAuthDto : BaseAuthDto
{
    public new string Provider { get; set; } = "google";
}
