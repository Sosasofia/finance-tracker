namespace FinanceTracker.Server.Models.DTOs.Response
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public List<string>? Details { get; set; }

        public ErrorResponse(string message, List<string>? details = null)
        {
            Message = message;
            Details = details;
        }
    }
}
