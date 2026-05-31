using System;
using Domain.Value_object;

namespace Domain.Entity;

public class Location(string name, Address address)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = name;
    public Address Address { get; private set; } = address;
}
