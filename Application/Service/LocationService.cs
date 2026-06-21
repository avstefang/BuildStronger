using Application.Dto;
using Application.Interface;
using Application.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class LocationService(ILocationRepository locationRepository)
{
    private readonly ILocationRepository _locationRepository = locationRepository;

    public async Task<IEnumerable<GetLocationDto>> GetAllLocationsAsync() =>
        (await _locationRepository.GetAllLocationsAsync())?.Select(location => location.ToDto()) ?? [];

    public async Task<GetLocationDto?> GetLocationByIdAsync(Guid id) =>
        (await _locationRepository.GetLocationByIdAsync(id))?.ToDto();
}