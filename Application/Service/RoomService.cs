using Application.Interface;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class RoomService(IRoomRepository roomRepository)
{
    private readonly IRoomRepository _roomRepository = roomRepository;

    public async Task<IEnumerable<Room>?> GetAllRoomsAsync() =>
        await _roomRepository.GetAllRoomsAsync();

    public async Task<Room?> GetRoomByIdAsync(Guid id) =>
        await _roomRepository.GetRoomByIdAsync(id);
}