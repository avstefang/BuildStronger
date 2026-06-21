using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context_model;

public class EquipmentDbContext(DbContextOptions<EquipmentDbContext> options) : DbContext(options)
{
    public DbSet<Equipment> Equipment { get; set; }
}