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
        // No-tracking: this read feeds the capacity check in ReserveLessonAsync. If the
        // included Lesson were tracked, attaching the (different) Lesson instance that comes
        // from LessonDbContext in AddReservationAsync would throw an identity conflict.
        return await ReservationsWithIncludes()
            .AsNoTracking()
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
            .Where(r => r.AthleteId ==
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

    // Accepted reservations per lesson for today onwards, in one grouped query, so the planning can
    // show real availability. Past (stale) reservations are excluded so they don't keep "filling" a
    // recurring lesson. Keyed by lesson id; lessons with no bookings are simply absent from the map.
    public async Task<Dictionary<Guid, int>> GetAcceptedCountsByLessonAsync()
    {
        return await DbContext.Set<Reservation>()
            .Where(r => r.Status == ReservationStatus.Accepted && r.ReservationDate >= DateTime.Today)
            .GroupBy(r => r.Lesson.Id)
            .Select(g => new { LessonId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.LessonId, g => g.Count);
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
        // AthleteId is set in the Reservation constructor; the Athlete navigation stays ignored here.

        // Lesson comes from LessonDbContext — attach it (cascades to Workout, Room, Schedule)
        if (DbContext.Entry(reservation.Lesson).State == EntityState.Detached)
            DbContext.Attach(reservation.Lesson);

        await AddAsync(reservation);
    }

    public async Task UpdateReservationAsync(Reservation reservation) => await UpdateAsync(reservation);

    public async Task DeleteReservationAsync(Guid id)
    {
        Reservation? reservation = await GetReservationByIdAsync(id);
        if (reservation == null) return;

        DbContext.Set<Reservation>().Remove(reservation);
        await DbContext.SaveChangesAsync();
    }
}
