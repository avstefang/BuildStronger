using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context_model;

public class SubscriptionPlanDbContext(DbContextOptions<SubscriptionPlanDbContext> options) : DbContext(options)
{
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubscriptionPlan>(builder =>
        {
            builder.Property(sp => sp.Name).HasColumnName("name");
            builder.Property(sp => sp.Price).HasColumnName("price");
            builder.Property(sp => sp.DurationInMonths).HasColumnName("durationInMonths");
            builder.Property(sp => sp.PaymentMethod).HasColumnName("paymentMethod").HasConversion<string>();
            builder.Property(sp => sp.Currency).HasColumnName("paymentCurrency").HasConversion<string>();
        });
    }
}