using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Server.Models
{
    public class Context : DbContext
    {
        public Context() { }

        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Reimburstment> Reimburstments { get; set; }
        public DbSet<Installment> Installments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reimburstment>()
                .HasOne(r => r.Transaction)
                .WithOne(t => t.Reimburstment)
                .HasForeignKey<Reimburstment>(r => r.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Installment>()
                .HasOne(i => i.Transaction)
                .WithMany(t => t.InstallmentsList)
                .HasForeignKey(i => i.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

           modelBuilder.Entity<Reimburstment>()
                .Property(r => r.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Installment>()
                .Property(r => r.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Transaction>().ToTable("transaction");
            modelBuilder.Entity<Category>().ToTable("category");
            modelBuilder.Entity<PaymentMethod>().ToTable("payment_method");
            modelBuilder.Entity<Reimburstment>().ToTable("reimburstment");
            modelBuilder.Entity<Installment>().ToTable("installment");
        }
    }
}
