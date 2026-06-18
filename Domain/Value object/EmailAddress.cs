using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

        string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
        Regex emailRegex = new(pattern, RegexOptions.Compiled);
        if (!emailRegex.IsMatch(address))
        {
            throw new DomainException("Email address is not valid.", nameof(address));
        }

        Address = address;
    }
}