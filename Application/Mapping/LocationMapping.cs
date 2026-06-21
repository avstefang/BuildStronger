using Application.Dto;
using Domain.Entity;
using Domain.Value_object;

namespace Application.Mapping;

public static class LocationMapping
{
    public static GetLocationDto ToDto(this Location location) => new()
    {
        Id = location.Id,
        Name = location.Name,
        Address = location.Address.ToDto()
    };

    public static GetAddressDto ToDto(this Address address) => new()
    {
        Street = address.Street,
        HouseNumber = address.HouseNumber,
        City = address.City,
        ZipCode = address.ZipCode,
        DisplayString = address.ToDisplayString()
    };
}
