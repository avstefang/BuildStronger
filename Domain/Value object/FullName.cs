using Domain.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Value_object;

public sealed record FullName
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be null or empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be null or empty.", nameof(lastName));
        FirstName = firstName;
        LastName = lastName;
    }

    public override string ToString() => $"{FirstName} {LastName}";
}