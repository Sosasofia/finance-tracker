namespace FinanceTracker.Application.Common.DTOs;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public List<string> Errors { get; } = new List<string>();

    private Result(bool isSuccess, T? value, List<string>? errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        if (errors != null)
        {
            Errors = errors;
        }
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null);
    }

    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T>(false, default, errors);
    }
}
