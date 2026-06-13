using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context_model;

public class SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : DbContext(options)
{
    public DbSet<Subscription> Subscription { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>( builder =>
        {
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
            builder.Property(sp => sp.Price).HasColumnName("price");
            builder.Property(sp => sp.DurationInMonths).HasColumnName("durationInMonths");
            builder.Property(sp => sp.MonthlyCreditAmount).HasColumnName("monthlyCreditAmount");
            builder.Property(sp => sp.PaymentMethod).HasColumnName("paymentMethod").HasConversion<string>();
            builder.Property(sp => sp.Currency).HasColumnName("paymentCurrency").HasConversion<string>();
        });
    }
}