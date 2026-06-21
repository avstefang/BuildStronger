namespace Application.Dto;

/// <summary>
/// A confirmed participant of a lesson as shown to fellow members: username and profile
/// photo only. Deliberately excludes email and other personal detail.
/// </summary>
public class GetLessonParticipantDto
{
    public string Username { get; init; } = string.Empty;

    /// <summary>Server filename of the profile photo (null when none). Served by GET api/Photo/{file}.</summary>
    public string? PhotoFile { get; init; }
}
