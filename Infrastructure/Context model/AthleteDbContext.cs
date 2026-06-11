using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context_model;

public class AthleteDbContext(DbContextOptions<AthleteDbContext> options) : DbContext(options)
{
    public DbSet<Athlete> Athletes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Athlete>(builder =>
        {
            builder.OwnsOne(a => a.EmailAddress, e =>
            {
                e.Property(ea => ea.Address).HasColumnName("EmailAddress");
            });
            builder.OwnsOne(a => a.FullName, fn =>
            {
                fn.Property(f => f.FirstName).HasColumnName("FirstName");
                fn.Property(f => f.LastName).HasColumnName("LastName");
            });
            builder.OwnsOne(a => a.PhotoPath, pp =>
            {
                pp.Property(p => p.Path).HasColumnName("PhotoPath");
            });
        });
    }
}