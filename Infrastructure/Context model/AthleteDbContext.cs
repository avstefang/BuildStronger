using Domain.Entity;
using Infrastructure.Conversion;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context_model;

public class AthleteDbContext(DbContextOptions<AthleteDbContext> options) : DbContext(options)
{
    public DbSet<Athlete> Athlete { get; set; }
    public DbSet<Subscription> Subscription { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Athlete>(builder =>
        {
            builder.ToTable("athlete");
            builder.HasMany(a => a.Subscriptions)
                   .WithOne()
                   .HasForeignKey("AthleteId");
            builder.OwnsOne(a => a.EmailAddress, e =>
            {
                e.Property(ea => ea.Address).HasColumnName("emailAddress");
            });
            builder.OwnsOne(a => a.FullName, fn =>
            {
                fn.Property(f => f.FirstName).HasColumnName("firstName");
                fn.Property(f => f.LastName).HasColumnName("lastName");
            });
            builder.Property(a => a.Role).HasColumnName("role").HasConversion<string>();
            builder.OwnsOne(a => a.PhotoPath, pp =>
            {
                pp.Property(p => p.Path).HasColumnName("photoPath");
            });
        });

        modelBuilder.Entity<Subscription>(builder =>
        {
            builder.ToTable("subscription");
            builder.Property<Guid>("AthleteId").HasColumnName("athleteId");
            builder.HasOne(s => s.SubscriptionPlan)
                   .WithMany()
                   .HasForeignKey("SubscriptionPlanId");
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
            builder.Property(sp => sp.WeeklyCreditAmount).HasColumnName("weeklyCreditAmount");
            builder.Property(sp => sp.Currency).HasColumnName("paymentCurrency").HasConversion<string>();
            builder.Property(sp => sp.PaymentMethods).HasColumnName("paymentMethod")
                   .HasConversion(PaymentMethodConversion.Converter, PaymentMethodConversion.Comparer);
        });
    }
}
