using Application.Interface;
using Domain.Entity;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class LocationRepository(LocationDbContext dbContext) : Repository<Location, Guid>(dbContext), ILocationRepository
{
    public async Task<Location> GetLocationByIdAsync(Guid locationId)
    {
        return await DbContext.Set<Location>().FindAsync(locationId)
            ?? throw new InvalidOperationException($"Location with ID {locationId} not found.");
    }

    public async Task<Location> GetLocationByNameAsync(string locationName)
    {
        return await DbContext.Set<Location>().FirstOrDefaultAsync(l => l.Name == locationName)
            ?? throw new InvalidOperationException($"Location with name '{locationName}' not found.");
    }

    public async Task<IEnumerable<Location>> GetAllLocationsAsync()
    {
        return await DbContext.Set<Location>().ToListAsync();
    }

    public async Task AddLocationAsync(Location location) => await AddAsync(location);

    public async Task UpdateLocationAsync(Location location) => await UpdateAsync(location);

    public async Task DeleteLocationAsync(Guid locationId) => await DeleteAsync(locationId);
}
