using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Reimbursement> Reimbursements { get; set; }
    public DbSet<Installment> Installments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(builder =>
        {
            builder.ToTable("transaction");
            builder.HasKey(t => t.Id);
            builder.HasQueryFilter(t => !t.IsDeleted);

            builder.OwnsOne(t => t.Money, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.PaymentMethod)
                .WithMany()
                .HasForeignKey(t => t.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .IsRequired();
        });

        modelBuilder.Entity<Installment>(builder =>
        {
            builder.ToTable("installment");
            builder.HasKey(i => i.Id);

            builder.OwnsOne(i => i.Money, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        modelBuilder.Entity<User>().ToTable("user");
        modelBuilder.Entity<Category>().ToTable("category");
        modelBuilder.Entity<PaymentMethod>().ToTable("payment_method");

        modelBuilder.Entity<Reimbursement>(builder =>
        {
            builder.ToTable("reimbursement");
            builder.HasKey(r => r.Id);

            builder.OwnsOne(r => r.Money, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.HasOne(r => r.Transaction)
                .WithOne(t => t.Reimbursement)
                .HasForeignKey<Reimbursement>(r => r.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
