namespace Application.Dto;

public class GetAddressDto
{
    public string Street { get; init; } = string.Empty;
    public string HouseNumber { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string DisplayString { get; init; } = string.Empty;
}
