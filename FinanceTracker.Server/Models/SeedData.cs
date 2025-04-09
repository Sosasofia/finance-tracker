using FinanceTracker.Server.Models.Entities;

namespace FinanceTracker.Server.Models
{
    public static class SeedData
    {
        public static void Initialize(Context context)
        {
            // Evitá sembrar de nuevo si ya existen
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Id = Guid.NewGuid(), Name = "Comida" },
                    new Category { Id = Guid.NewGuid(), Name = "Transporte" },
                    new Category { Id = Guid.NewGuid(), Name = "Salud" }
                );
            }

            if (!context.PaymentMethods.Any())
            {
                context.PaymentMethods.AddRange(
                    new PaymentMethod { Id = Guid.NewGuid(), Name = "Supervielle", Type = "Debito" },
                    new PaymentMethod { Id = Guid.NewGuid(), Name = "Provincia", Type = "Debito" }
                );
            }

            context.SaveChanges();
        }
    }
}
