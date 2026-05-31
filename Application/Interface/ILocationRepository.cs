using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> GetLocationAsync(Location location);
    Task<IEnumerable<Location>> GetAllLocationsAsync();
    Task AddLocationAsync(Location location);
    Task UpdateLocationAsync(Location location);
    Task DeleteLocationAsync(Location location);
}