using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Infrastructure.Persistance;

public static class SeedInitialData
{
    public static async Task Initialize(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Id = Guid.NewGuid(), Name = "Food" },
                new Category { Id = Guid.NewGuid(), Name = "Transportation" },
                new Category { Id = Guid.NewGuid(), Name = "Health" },
                new Category { Id = Guid.NewGuid(), Name = "Entertainment" },
                new Category { Id = Guid.NewGuid(), Name = "Housing" },
                new Category { Id = Guid.NewGuid(), Name = "Utilities" }
            );
        }


        if (!context.PaymentMethods.Any())
        {
            context.PaymentMethods.AddRange(
                new PaymentMethod { Id = Guid.NewGuid(), Name = "Cash", Type = "Cash" },
                new PaymentMethod { Id = Guid.NewGuid(), Name = "Visa", Type = "Credit" },
                new PaymentMethod { Id = Guid.NewGuid(), Name = "Mastercard", Type = "Credit" },
                new PaymentMethod { Id = Guid.NewGuid(), Name = "Bank Debit Card", Type = "Debit" }
            );
        }

        await context.SaveChangesAsync();
    }
}
