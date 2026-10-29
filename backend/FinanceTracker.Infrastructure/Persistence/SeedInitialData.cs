using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Persistence;

namespace FinanceTracker.Infrastructure;

public static class SeedInitialData
{
    public static async Task Initialize(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                Category.CreateDefault("Food"),
                Category.CreateDefault("Transportation"),
                Category.CreateDefault("Health"),
                Category.CreateDefault("Entertainment"),
                Category.CreateDefault("Housing"),
                Category.CreateDefault("Utilities"),
                Category.CreateDefault("Salary")
            );
        }

        if (!context.PaymentMethods.Any())
        {
            context.PaymentMethods.AddRange(
                PaymentMethod.CreateDefault("Cash", "Cash"),
                PaymentMethod.CreateDefault("Visa", "Credit"),
                PaymentMethod.CreateDefault("Mastercard", "Credit"),
                PaymentMethod.CreateDefault("Bank Debit Card", "Debit")
            );
        }

        await context.SaveChangesAsync();
    }
}
