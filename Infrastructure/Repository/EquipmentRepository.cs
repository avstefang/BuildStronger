using Application.Interface;
using Domain.Entity;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class EquipmentRepository(LessonDbContext dbContext) : Repository<Equipment, Guid>(dbContext), IEquipmentRepository
{
    public async Task<Equipment> GetEquipmentByIdAsync(Guid id)
    {
        return await DbContext.Set<Equipment>().FindAsync(id)
            ?? throw new InvalidOperationException($"Equipment with ID {id} not found.");
    }

    public async Task<Equipment> GetEquipmentByNameAsync(string equipmentName)
    {
        return await DbContext.Set<Equipment>().FirstOrDefaultAsync(e => e.Name == equipmentName)
            ?? throw new InvalidOperationException($"Equipment with name '{equipmentName}' not found.");
    }

    public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync()
    {
        return await DbContext.Set<Equipment>().ToListAsync();
    }

    public async Task AddEquipmentAsync(Equipment equipment) => await AddAsync(equipment);

    public async Task DeleteEquipmentAsync(Guid equipmentId) => await DeleteAsync(equipmentId);
}
