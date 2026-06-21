namespace Application.Dto;

public class GetRoomDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Capacity { get; init; }
    public Guid LocationId { get; init; }
}
