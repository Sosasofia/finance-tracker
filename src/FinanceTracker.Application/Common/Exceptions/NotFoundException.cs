namespace FinanceTracker.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' with id: ({key}) was not found.")
    {
    }
}
