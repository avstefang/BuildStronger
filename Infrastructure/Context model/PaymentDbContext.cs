using Domain.Entity;
using Infrastructure.Conversion;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context_model;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
{
    public DbSet<Payment> Payment { get; set; }
    public DbSet<Subscription> Subscription { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(builder =>
        {
            builder.ToTable("payment");
            builder.Property(p => p.Amount).HasColumnName("amount");
            builder.Property(p => p.Currency).HasColumnName("currency").HasConversion<string>();
            builder.Property(p => p.Method).HasColumnName("method").HasConversion<string>();
            builder.Property(p => p.PayedAt).HasColumnName("payedAt");
            builder.Property(p => p.Status).HasColumnName("status").HasConversion<string>();
            builder.OwnsOne(p => p.ProcessorId, pi =>
            {
                pi.Property(p => p.Id).HasColumnName("processorId").HasConversion<string>();
            });
            builder.HasOne(p => p.Subscription)
                   .WithMany()
                   .HasForeignKey("subscriptionId");
        });

        modelBuilder.Entity<Subscription>(builder =>
        {
            builder.ToTable("subscription");
            builder.HasOne(s => s.SubscriptionPlan)
                   .WithMany()
                   .HasForeignKey("subscriptionPlanId");
            builder.Property<Guid>("AthleteId").HasColumnName("athleteId");
            builder.Property<Guid>("SubscriptionPlanId").HasColumnName("subscriptionPlanId");
            builder.Property(s => s.StartDate).HasColumnName("startDate");
            builder.Property(s => s.Status).HasColumnName("status").HasConversion<string>();
            builder.Property(s => s.AutoRenew).HasColumnName("autoRenew");
        });

        modelBuilder.Entity<SubscriptionPlan>(builder =>
        {
            builder.ToTable("subscriptionPlan");
            builder.Property(sp => sp.Name).HasColumnName("name");
            builder.Property(sp => sp.Price).HasColumnName("price");
            builder.Property(sp => sp.DurationInMonths).HasColumnName("durationInMonths");
            builder.Property(sp => sp.MonthlyCreditAmount).HasColumnName("monthlyCreditAmount");
            builder.Property(sp => sp.PaymentMethods).HasColumnName("paymentMethod")
                   .HasConversion(PaymentMethodConversion.Converter, PaymentMethodConversion.Comparer);
            builder.Property(sp => sp.Currency).HasColumnName("paymentCurrency").HasConversion<string>();
        });
    }
}
