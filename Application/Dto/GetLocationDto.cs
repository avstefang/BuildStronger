namespace Application.Dto;

public class GetLocationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public GetAddressDto Address { get; init; } = new();
}
