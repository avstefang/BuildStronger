using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class EquipmentMapping
{
    public static GetEquipmentDto ToDto(this Equipment equipment) => new()
    {
        Id = equipment.Id,
        Name = equipment.Name
    };
}
