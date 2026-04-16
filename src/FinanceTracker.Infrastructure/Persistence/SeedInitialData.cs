using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
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
                Category.Create("Food", CategoryType.Standard, null),
                Category.Create("Transportation", CategoryType.Standard, null),
                Category.Create("Health", CategoryType.Standard, null),
                Category.Create("Entertainment", CategoryType.Standard, null),
                Category.Create("Housing", CategoryType.Standard, null),
                Category.Create("Utilities", CategoryType.Standard, null),
                Category.Create("Salary", CategoryType.Standard, null)
            );
        }

        if (!context.PaymentMethods.Any())
        {
            context.PaymentMethods.AddRange(
                PaymentMethod.Create("Cash", "Cash", null),
                PaymentMethod.Create("Visa", "Credit", null),
                PaymentMethod.Create("Mastercard", "Credit", null),
                PaymentMethod.Create("Bank Debit Card", "Debit", null)
            );
        }

        await context.SaveChangesAsync();
    }
}
