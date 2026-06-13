using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context_model;

public class EquipmentRoomDbContext(DbContextOptions<EquipmentRoomDbContext> options) : DbContext(options)
{
    public DbSet<EquipmentRoom> EquipmentRoom { get; set; }
    public DbSet<Room> Room { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EquipmentRoom>(builder =>
        {
            builder.HasOne(er => er.Room).WithMany();
            builder.Property<Guid>("EquipmentId").HasColumnName("equipmentId");
            builder.Property<Guid>("RoomId").HasColumnName("roomId");
        });
        modelBuilder.Entity<Room>(builder =>
        {
            builder.HasOne<Room>().WithMany();
        });
    }
}