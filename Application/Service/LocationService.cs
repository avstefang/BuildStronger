using Application.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class LocationService(ILocationRepository locationRepository)
{
    private readonly ILocationRepository _locationRepository = locationRepository;

    public async Task<IEnumerable<Domain.Entity.Location>> GetAllLocationsAsync() =>
        await _locationRepository.GetAllLocationsAsync();

    public async Task<Domain.Entity.Location?> GetLocationByIdAsync(Guid id) =>
        await _locationRepository.GetLocationByIdAsync(id);
}