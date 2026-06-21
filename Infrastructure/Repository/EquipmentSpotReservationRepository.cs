using Application.Interface;
using Domain.Entity;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class EquipmentSpotReservationRepository(EquipmentSpotDbContext dbContext)
    : Repository<EquipmentSpotReservation, Guid>(dbContext), IEquipmentSpotReservationRepository
{
    public async Task AddEquipmentSpotReservationAsync(EquipmentSpotReservation equipmentSpotReservation)
    {
        // The chosen spot already exists (seeded per room layout) — attach it so EF links rather than inserts.
        if (DbContext.Entry(equipmentSpotReservation.EquipmentSpot).State == EntityState.Detached)
            DbContext.Attach(equipmentSpotReservation.EquipmentSpot);

        await AddAsync(equipmentSpotReservation);
    }

    public async Task<IEnumerable<EquipmentSpotReservation>?> GetByLessonIdAsync(Guid lessonId)
    {
        return await DbContext.Set<EquipmentSpotReservation>()
            .AsNoTracking()
            .Include(e => e.EquipmentSpot)
            .Where(e => e.LessonId == lessonId)
            .ToListAsync();
    }

    public async Task DeleteByReservationIdAsync(Guid reservationId)
    {
        // No-op when the reservation has no spot (non-equipment lessons), so cancel stays safe.
        EquipmentSpotReservation? entity = await DbContext.Set<EquipmentSpotReservation>()
            .FirstOrDefaultAsync(e => e.ReservationId == reservationId);
        if (entity is null)
            return;

        DbContext.Set<EquipmentSpotReservation>().Remove(entity);
        await DbContext.SaveChangesAsync();
    }
}
