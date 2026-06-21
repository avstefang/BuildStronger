using Domain.Entity;
using Domain.Value_object;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context_model;

public class LocationDbContext(DbContextOptions<LocationDbContext> options) : DbContext(options)
{
    public DbSet<Location> Location { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(builder =>
        {
            builder.ToTable("location");
            builder.Property(l => l.Name).HasColumnName("name");
            builder.Property(l => l.Address)
                   .HasColumnName("address")
                   .HasConversion(
                       a => a.Get(),
                       s => new Address(s)
                   );
        });
    }
}