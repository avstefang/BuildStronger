using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure;

public class DbConnection : DbContext
{
    public DbSet<Athlete> Athlete { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<EquipmentRoom> EquipmentRoom { get; set; }
    public DbSet<EquipmentSpot> EquipmentSpot { get; set; }
    public DbSet<Lesson> Lesson { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Payment> Payment { get; set; }
    public DbSet<Reservation> Reservation { get; set; }
    public DbSet<Room> Room { get; set; }
    public DbSet<Schedule> Schedule { get; set; }
    public DbSet<Subscription> Subscription { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }
    public DbSet<Workout> Workout { get; set; }
    
    public DbConnection(DbContextOptions<DbConnection> options) : base(options)
    {
    }
}