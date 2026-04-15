namespace FinanceTracker.Domain.Exceptions;

public class InUseException : DomainException
{
    public InUseException(Guid entiyId, string entity)
        : base($"Cannot delete {entity} with id: '{entiyId}' because it is currently referenced by one or more transactions.")
    {
    }
}
