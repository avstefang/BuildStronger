using Application.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class EquipmentRoomService(IEquipmentRoomRepository equipmentRoomRepository)
{
    private readonly IEquipmentRoomRepository _equipmentRoomRepository = equipmentRoomRepository;
    public async Task<IEnumerable<Domain.Entity.EquipmentRoom>> GetAllEquipmentRoomsAsync() =>
        await _equipmentRoomRepository.GetAllEquipmentRoomsAsync();
    public async Task<Domain.Entity.EquipmentRoom?> GetEquipmentRoomByIdAsync(Guid id) =>
        await _equipmentRoomRepository.GetEquipmentRoomByIdAsync(id);
}