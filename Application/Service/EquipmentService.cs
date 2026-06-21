using Application.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class EquipmentService(IEquipmentRepository equipmentRepository)
{
    private readonly IEquipmentRepository _equipmentRepository = equipmentRepository;

    public async Task<IEnumerable<Domain.Entity.Equipment>> GetAllEquipmentAsync() =>
        await _equipmentRepository.GetAllEquipmentAsync();

    public async Task<Domain.Entity.Equipment?> GetEquipmentByIdAsync(Guid id) =>
        await _equipmentRepository.GetEquipmentByIdAsync(id);
}