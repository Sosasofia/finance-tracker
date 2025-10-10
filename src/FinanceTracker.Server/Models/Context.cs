using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Server.Models;

public class Context : DbContext
{
    public Context() { }

    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Reimbursement> Reimbursements { get; set; }
    public DbSet<Installment> Installments { get; set; }
    public DbSet<CustomCategory> CustomCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>().HasQueryFilter(t => !t.IsDeleted);

        modelBuilder.Entity<Reimbursement>()
            .HasOne(r => r.Transaction)
            .WithOne(t => t.Reimbursement)
            .HasForeignKey<Reimbursement>(r => r.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Installment>()
            .HasOne(i => i.Transaction)
            .WithMany(t => t.InstallmentsList)
            .HasForeignKey(i => i.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reimbursement>()
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
        modelBuilder.Entity<Reimbursement>().ToTable("reimbursement");
        modelBuilder.Entity<Installment>().ToTable("installment");
        modelBuilder.Entity<CustomCategory>().ToTable("custom_category");
    }
}
