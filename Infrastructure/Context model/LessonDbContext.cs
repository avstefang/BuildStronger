using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context_model;

public class LessonDbContext(DbContextOptions<LessonDbContext> contextOptions) : DbContext(contextOptions)
{
    public DbSet<Lesson> Lesson { get; set; }
    public DbSet<Workout> Workout { get; set; }
    public DbSet<Schedule> Schedule { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Room> Room { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>(builder =>
        {
            builder.ToTable("lesson");
            builder.Property(l => l.MaxCapacity).HasColumnName("maxCapacity");
            builder.Property(l => l.CustomDuration).HasColumnName("customDuration");
            builder.Property<Guid?>("InstructorId").HasColumnName("instructorId").IsRequired(false);
            builder.Ignore(l => l.Instructor);
            builder.HasOne(l => l.Workout)
                   .WithMany()
                   .HasForeignKey("workoutId");
            builder.HasOne(l => l.Room)
                   .WithMany()
                   .HasForeignKey("roomId");
            builder.HasOne(l => l.Schedule)
                   .WithOne()
                   .HasForeignKey<Schedule>("LessonId");
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
            builder.HasMany(w => w.Equipment)
                   .WithMany()
                   .UsingEntity(j => j.ToTable("workout_equipment"));
        });

        modelBuilder.Entity<Equipment>(builder =>
        {
            builder.ToTable("equipment");
            builder.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Room>(builder =>
        {
            builder.ToTable("room");
            builder.Property(r => r.Name).HasColumnName("name");
            builder.Property(r => r.Capacity).HasColumnName("capacity");
            builder.Property(r => r.LocationId).HasColumnName("locationId");
        });
    }
}
