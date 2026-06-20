using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context_model;

public class EquipmentSpotDbContext(DbContextOptions<EquipmentSpotDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<EquipmentSpot> EquipmentSpot { get; set; }
    public DbSet<EquipmentSpotReservation> EquipmentSpotReservation { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<EquipmentRoom> EquipmentRoom { get; set; }
    public DbSet<Lesson> Lesson { get; set; }
    public DbSet<Reservation> Reservation { get; set; }
    public DbSet<Schedule> Schedule { get; set; }
    public DbSet<Workout> Workout { get; set; }
    public DbSet<Room> Room { get; set; }
    public DbSet<Athlete> Athlete { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EquipmentSpot>(builder =>
        {
            builder.ToTable("equipmentSpot");
            builder.OwnsOne(e => e.EquipmentPosition, pos =>
            {
                pos.Property(p => p.RowNumber).HasColumnName("rowNumber");
                pos.Property(p => p.SpotNumber).HasColumnName("spotNumber");
            });
            builder.HasOne(e => e.Equipment)
                   .WithMany()
                   .HasForeignKey("equipmentId");
            builder.HasOne(e => e.EquipmentRoom)
                   .WithMany()
                   .HasForeignKey("equipmentRoomId");
        });

        modelBuilder.Entity<EquipmentSpotReservation>(builder =>
        {
            builder.ToTable("equipmentSpot_reservation");
            builder.HasKey(e => e.ReservationId);
            builder.Property(e => e.ReservationId).HasColumnName("reservationId");
            builder.Property(e => e.LessonId).HasColumnName("lessonId");
            builder.Property(e => e.AthleteId).HasColumnName("athleteId");
            builder.Property(e => e.ReservedAt).HasColumnName("reservedAt");
            builder.HasOne(e => e.EquipmentSpot)
                   .WithMany()
                   .HasForeignKey("equipmentSpotId");
        });

        modelBuilder.Entity<Equipment>(builder =>
        {
            builder.ToTable("equipment");
            builder.Property(e => e.Name).HasColumnName("name");
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

        modelBuilder.Entity<Lesson>(builder =>
        {
            builder.ToTable("lesson");
            builder.Property(l => l.MaxCapacity).HasColumnName("maxCapacity");
            builder.Property(l => l.CustomDuration).HasColumnName("customDuration");
            builder.Property(l => l.InstructorId).HasColumnName("instructorId");
            builder.HasOne(l => l.Workout)
                   .WithMany()
                   .HasForeignKey("workoutId");
            builder.HasOne(l => l.Schedule)
                   .WithOne()
                   .HasForeignKey<Schedule>("LessonId");
            builder.HasOne(l => l.Room)
                   .WithMany()
                   .HasForeignKey("roomId");
            builder.Ignore(l => l.Instructor);
        });

        modelBuilder.Entity<Reservation>(builder =>
        {
            builder.ToTable("reservation");
            builder.Property(r => r.ReservationDate).HasColumnName("reservationDate");
            builder.Property(r => r.ReservedAt).HasColumnName("reservedAt");
            builder.Property(r => r.Status).HasColumnName("status").HasConversion<string>();
            builder.Property<bool>("IsCheckedIn").HasColumnName("isCheckedIn");
            builder.Property(r => r.AthleteId).HasColumnName("athleteId");
            builder.HasOne(r => r.Lesson)
                   .WithMany()
                   .HasForeignKey("lessonId");
            builder.Ignore(r => r.Athlete);
            // The spot link is modelled separately as EquipmentSpotReservation, not on the reservation row.
            builder.Ignore(r => r.EquipmentSpot);
        });

        modelBuilder.Entity<Schedule>(builder =>
        {
            builder.ToTable("schedule");
            builder.Property(s => s.StartTime).HasColumnName("startTime");
            builder.Property(s => s.StartDay).HasColumnName("startDay").HasConversion<string>();
            builder.Property(s => s.RegisteredAt).HasColumnName("registerDate");
            builder.Property<Guid>("LessonId").HasColumnName("lessonId");
            builder.OwnsOne(s => s.Repetition, rep =>
            {
                rep.Property(r => r.RepetitionCount).HasColumnName("repetitionCount");
                rep.Property(r => r.RepetitionEndDate).HasColumnName("repetitionEndDate");
            });
        });

        modelBuilder.Entity<Workout>(builder =>
        {
            builder.ToTable("workout");
            builder.Property(w => w.Name).HasColumnName("name");
            builder.Property(w => w.Description).HasColumnName("description");
            builder.OwnsOne(w => w.Duration, dur =>
            {
                dur.Property(d => d.Minutes).HasColumnName("duration");
            });
            builder.Ignore(w => w.Equipment);
        });

        modelBuilder.Entity<Room>(builder =>
        {
            builder.ToTable("room");
            builder.Property(r => r.Name).HasColumnName("name");
            builder.Property(r => r.Capacity).HasColumnName("capacity");
            builder.Property(r => r.LocationId).HasColumnName("locationId");
        });

        modelBuilder.Entity<Athlete>(builder =>
        {
            builder.ToTable("athlete");
            builder.OwnsOne(a => a.EmailAddress, e =>
            {
                e.Property(ea => ea.Address).HasColumnName("emailAddress");
            });
            builder.OwnsOne(a => a.FullName, fn =>
            {
                fn.Property(f => f.FirstName).HasColumnName("firstName");
                fn.Property(f => f.LastName).HasColumnName("lastName");
            });
            builder.Property(a => a.Role).HasColumnName("role").HasConversion<string>();
            builder.OwnsOne(a => a.PhotoPath, pp =>
            {
                pp.Property(p => p.Path).HasColumnName("photoPath");
            });
        });
    }
}
