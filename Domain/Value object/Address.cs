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

    public string Get()
    {
        StringBuilder address = new StringBuilder();
        address.Append(Street);
        address.Append(" ");
        address.Append(HouseNumber);
        address.Append(", ");
        address.Append(City);
        address.Append(" ");
        address.Append(ZipCode);
        return address.ToString();
    }
}