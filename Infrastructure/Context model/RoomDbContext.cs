using Domain.Entity;
using Domain.Value_object;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context_model;

public class RoomDbContext(DbContextOptions<RoomDbContext> options) : DbContext(options)
{
    public DbSet<Room> Room { get; set; }
    public DbSet<EquipmentRoom> EquipmentRoom { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(builder =>
        {
            builder.ToTable("room");
            builder.Property(r => r.Name).HasColumnName("name");
            builder.Property(r => r.Capacity).HasColumnName("capacity");
            builder.Property(r => r.LocationId).HasColumnName("locationId");
        });

        modelBuilder.Entity<EquipmentRoom>(builder =>
        {
            builder.ToTable("equipmentRoom");
            builder.HasOne(e => e.Room)
                   .WithMany()
                   .HasForeignKey("roomId");
            builder.OwnsOne(e => e.MaxSpot, layout =>
            {
                layout.Property(l => l.RowCount).HasColumnName("rowCount");
                layout.Property(l => l.SpotsPerRow).HasColumnName("spotsPerRow");
            });
        });
    }
}
