namespace FinanceTracker.Application.Features;

public class Response<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public Response(T data)
    {
        Success = true;
        Data = data;
    }

    public Response(string message)
    {
        Success = false;
        Message = message;
    }
}
