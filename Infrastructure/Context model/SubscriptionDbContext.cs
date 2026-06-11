using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context_model;

public class SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : DbContext(options)
{
    public DbSet<Subscription> Subscriptions { get; set; }
}