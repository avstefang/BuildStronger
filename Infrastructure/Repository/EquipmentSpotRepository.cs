using Application.Interface;
using Domain.Entity;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class EquipmentSpotRepository(EquipmentSpotDbContext dbContext) : Repository<EquipmentSpot, Guid>(dbContext), IEquipmentSpotRepository
{
    private IQueryable<EquipmentSpot> SpotsWithIncludes() =>
        DbContext.Set<EquipmentSpot>()
            .Include(e => e.Equipment)
            .Include(e => e.EquipmentRoom).ThenInclude(er => er.Room);

    public async Task<EquipmentSpot?> GetEquipmentSpotByIdAsync(Guid equipmentSpotId)
    {
        return await SpotsWithIncludes().FirstOrDefaultAsync(e => e.Id == equipmentSpotId);
    }

    public async Task<EquipmentSpot?> GetEquipmentSpotByNameAsync(string equipmentName)
    {
        return await SpotsWithIncludes().FirstOrDefaultAsync(e => e.Equipment.Name == equipmentName);
    }

    public async Task<IEnumerable<EquipmentSpot>?> GetAllEquipmentSpotsAsync()
    {
        return await SpotsWithIncludes().ToListAsync();
    }

    public async Task AddEquipmentSpotAsync(EquipmentSpot equipmentSpot)
    {
        if (DbContext.Entry(equipmentSpot.Equipment).State == EntityState.Detached)
            DbContext.Attach(equipmentSpot.Equipment);
        if (DbContext.Entry(equipmentSpot.EquipmentRoom).State == EntityState.Detached)
            DbContext.Attach(equipmentSpot.EquipmentRoom);
        await AddAsync(equipmentSpot);
    }

    public async Task UpdateEquipmentSpotAsync(EquipmentSpot equipmentSpot) => await UpdateAsync(equipmentSpot);

    public async Task DeleteEquipmentSpotAsync(Guid equipmentSpotId) => await DeleteAsync(equipmentSpotId);
}
