using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Value_object;

public sealed record class Address
{
    public string Street { get; }
    public string HouseNumber { get; }
    public string City { get; }
    public string ZipCode { get; }

    public Address(string street, string houseNumber, string city, string zipCode)
    {
        Street = street;
        HouseNumber = houseNumber;
        City = city;
        ZipCode = zipCode;
    }

    public Address(string formatted)
    {
        var parts = formatted.Split('|');
        if (parts.Length != 4)
            throw new ArgumentException("Invalid address format. Expected format: 'Street|HouseNumber|City|ZipCode'.");

        Street = parts[0];
        HouseNumber = parts[1];
        City = parts[2];
        ZipCode = parts[3];
    }

    public string Get() => $"{Street}|{HouseNumber}|{City}|{ZipCode}";

    public string ToDisplayString() => $"{Street} {HouseNumber}, {ZipCode} {City}";
}