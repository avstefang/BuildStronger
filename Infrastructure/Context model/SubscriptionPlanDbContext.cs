using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context_model;

public class SubscriptionPlanDbContext(DbContextOptions<SubscriptionPlanDbContext> options) : DbContext(options)
{
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
}