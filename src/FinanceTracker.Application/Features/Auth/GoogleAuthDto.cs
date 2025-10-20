namespace FinanceTracker.Application.Features.Auth;

public class GoogleAuthDto : BaseAuthDto
{
    public new string Provider { get; set; } = "google";
}
