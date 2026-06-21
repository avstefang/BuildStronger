namespace Application.Dto;

/// <summary>
/// An instructor as shown in the staff portal: the underlying athlete's id, name, email and photo.
/// (An instructor is an athlete with the Instructor role.)
/// </summary>
public class GetInstructorDto
{
    public Guid Id { get; init; }
    public string Firstname { get; init; } = string.Empty;
    public string Lastname { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    /// <summary>Server filename of the profile photo (null when none). Served by GET api/Photo/{file}.</summary>
    public string? PhotoFile { get; init; }
    public string Username { get; init; } = string.Empty;
}
