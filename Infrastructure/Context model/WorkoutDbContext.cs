using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context_model;

public class WorkoutDbContext(DbContextOptions<WorkoutDbContext> options) : DbContext(options)
{
    public DbSet<Workout> Workout { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Workout>(builder =>
        {
            builder.Property(w => w.Name).HasColumnName("name");
            builder.Property(w => w.Description).HasColumnName("description");
            builder.Property(w => w.Duration).HasColumnName("duration");
        });
    }
}