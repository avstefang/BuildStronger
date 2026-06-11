using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface ILocationRepository : IRepository<Location, Guid>
{
    Task<Location> GetLocationByIdAsync(Guid locationId);
    Task<Location> GetLocationByNameAsync(string locationName);
    Task<IEnumerable<Location>> GetAllLocationsAsync();
    Task AddLocationAsync(Location location);
    Task UpdateLocationAsync(Location location);
    Task DeleteLocationAsync(Guid locationId);
}