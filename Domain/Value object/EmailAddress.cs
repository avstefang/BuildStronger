using System;
using System.Collections.Generic;
using System.Text;
using Domain.Exception;

namespace Domain.Value_object;

public sealed record class EmailAddress
{
    public string Address { get; }

    public EmailAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new DomainException("Email address cannot be empty.", nameof(address));
        }
        else if (address.Length < 5)
        {
            throw new DomainException("Input is too short to be a valid email address.", nameof(address));
        }
        else if (!address.Contains('@'))
        {
            throw new DomainException("Email address must contain '@' symbol.", nameof(address));
        }

        Address = address;
    }
}