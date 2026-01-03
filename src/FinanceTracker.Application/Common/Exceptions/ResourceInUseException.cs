namespace FinanceTracker.Application.Common.Exceptions;

public class ResourceInUseException : Exception
{
    public ResourceInUseException(string message) : base(message) { }
}
