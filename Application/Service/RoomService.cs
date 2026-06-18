using Application.Dto;
using Application.Interface;
using Application.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class RoomService(IRoomRepository roomRepository)
{
    private readonly IRoomRepository _roomRepository = roomRepository;

    public async Task<IEnumerable<GetRoomDto>> GetAllRoomsAsync() =>
        (await _roomRepository.GetAllRoomsAsync())?.Select(room => room.ToDto()) ?? [];

    public async Task<GetRoomDto?> GetRoomByIdAsync(Guid id) =>
        (await _roomRepository.GetRoomByIdAsync(id))?.ToDto();
}