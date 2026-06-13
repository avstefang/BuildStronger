using Domain.Entity;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context_model;

public class AthleteDbContext(DbContextOptions<AthleteDbContext> options) : DbContext(options)
{
    public DbSet<Athlete> Athlete { get; set; }
    public DbSet<Subscription> Subscription { get; private set; }
    public DbSet<SubscriptionPlan> SubscriptionPlan { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Athlete>(builder =>
        {
            builder.HasMany(s => s.Subscriptions).WithOne();
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
            builder.HasOne(s => s.SubscriptionPlan).WithMany();
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
            builder.Property(sp => sp.DurationInMonths).HasConversion<int>();
            builder.Property(sp => sp.MonthlyCreditAmount).HasConversion<int>();
            builder.Property(sp => sp.Price).HasColumnName("price").HasConversion<decimal>();
            builder.Property(sp => sp.Currency).HasColumnName("paymentCurrency").HasConversion<string>();
            builder.Property(sp => sp.PaymentMethod).HasColumnName("paymentMethod").HasConversion<string>();
        });
    }
}