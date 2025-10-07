namespace FinanceTracker.Server.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(Guid id);
    }
}
