using System;

namespace Domain.Entity;

public class Equipment(string name)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public required string Name { get; set; } = name;
}