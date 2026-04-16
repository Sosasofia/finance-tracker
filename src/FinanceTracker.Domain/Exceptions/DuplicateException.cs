namespace FinanceTracker.Domain.Exceptions;

public class DuplicateException : DomainException
{
    public DuplicateException(string message) : base(message)
    {
    }

    public static DuplicateException ForEntity(string entityName, string value)
        => new($"{entityName} with value '{value}' already exists.");
}
