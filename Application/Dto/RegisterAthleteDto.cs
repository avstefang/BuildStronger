using Domain.Value_object;

namespace Application.Dto;

public class RegisterAthleteDto(EmailAddress email, FullName fullName, string password)
{
    public EmailAddress Email { get; private set; } = email;
    public FullName FullName { get; private set; } = fullName;
    public string Password { get; private set; } = password;
}
