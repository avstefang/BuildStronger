using System;

namespace Domain.Entity;

public class Equipment(string name)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = name;
}