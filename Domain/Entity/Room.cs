using System;

namespace Domain.Entity;

public class Room(string name, int capacity, Guid locationId)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = name;
    public int Capacity { get; private set; } = capacity;
    public Guid LocationId { get; private set; } = locationId;
}
