using Application.Interface;
using Domain.Entity;
using Domain.Enum;
using Domain.Value_object;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class ReservationRepository(ReservationDbContext dbContext) : Repository<Reservation, Guid>(dbContext), IReservationRepository
{
    private IQueryable<Reservation> ReservationsWithIncludes() =>
        DbContext.Set<Reservation>()
            .Include(r => r.Lesson).ThenInclude(l => l.Workout)
            .Include(r => r.Lesson).ThenInclude(l => l.Schedule)
            .Include(r => r.Lesson).ThenInclude(l => l.Room);

    public async Task<Reservation?> GetReservationByIdAsync(Guid id)
    {
        return await ReservationsWithIncludes().FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Reservation>?> GetAllReservationsByLessonIdAsync(Guid lessonId)
    {
        return await ReservationsWithIncludes()
            .Where(r => r.Lesson.Id == lessonId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>?> GetAllWaitlistedReservationsByLessonIdAsync(Guid lessonId)
    {
        return await ReservationsWithIncludes()
            .Where(r => r.Lesson.Id == lessonId && r.Status == ReservationStatus.Waitinglist)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>?> GetAllReservationsByEmailAsync(EmailAddress email)
    {
        return await ReservationsWithIncludes()
            .Where(r => EF.Property<Guid>(r, "AthleteId") ==
                DbContext.Set<Athlete>()
                    .Where(a => a.EmailAddress.Address == email.Address)
                    .Select(a => a.Id)
                    .FirstOrDefault())
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>?> GetAllReservationsAsync()
    {
        return await ReservationsWithIncludes().ToListAsync();
    }

    public async Task<int> GetReservationSpotsLeftAsync(Guid lessonId)
    {
        Lesson? lesson = await DbContext.Set<Lesson>().FindAsync(lessonId);
        if (lesson == null) return 0;

        int accepted = await DbContext.Set<Reservation>()
            .CountAsync(r => r.Lesson.Id == lessonId && r.Status == ReservationStatus.Accepted);

        return Math.Max(0, lesson.MaxCapacity - accepted);
    }

    public async Task AddReservationAsync(Reservation reservation)
    {
        // Athlete is ignored in this context — set the FK shadow property manually
        DbContext.Entry(reservation).Property("AthleteId").CurrentValue = reservation.Athlete.Id;

        // Lesson comes from LessonDbContext — attach it (cascades to Workout, Room, Schedule)
        if (DbContext.Entry(reservation.Lesson).State == EntityState.Detached)
            DbContext.Attach(reservation.Lesson);

        await AddAsync(reservation);
    }

    public async Task UpdateReservationAsync(Reservation reservation) => await UpdateAsync(reservation);

    public async Task DeleteReservationAsync(Guid id) => await DeleteAsync(id);
}
