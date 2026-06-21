using Application.Dto;
using Application.Interface;
using Application.Mapping;
using Domain.Entity;
using Domain.Value_object;

namespace Application.Service;

/// <summary>
/// Manages instructors for the staff portal. An instructor is an athlete with the Instructor role,
/// so creating one adds a new athlete (with that role) and removing one demotes back to a member.
/// </summary>
public class InstructorService(
    IInstructorRepository instructorRepository,
    IAthleteRepository athleteRepository,
    IPasswordCrypt passwordCrypt)
{
    private readonly IInstructorRepository _instructorRepository = instructorRepository;
    private readonly IAthleteRepository _athleteRepository = athleteRepository;
    private readonly IPasswordCrypt _passwordCrypt = passwordCrypt;

    public async Task<IEnumerable<GetInstructorDto>> GetAllInstructorsAsync()
    {
        IEnumerable<Instructor> instructors = await _instructorRepository.GetAllInstructorsAsync();
        return instructors.Select(instructor => instructor.ToDto());
    }

    public async Task<GetInstructorDto> GetInstructorByIdAsync(Guid id)
    {
        Instructor instructor = await _instructorRepository.GetInstructorByIdAsync(id);
        return instructor.ToDto();
    }

    /// <summary>Creates a new instructor (an athlete with the Instructor role) from name + email.</summary>
    public async Task CreateInstructorAsync(CreateInstructorDto dto)
    {
        EmailAddress email = new(dto.Email);
        FullName fullName = new(dto.FirstName, dto.LastName);

        // Staff add trainers, who don't self-register, so seed a random password rather than asking for one.
        string passwordHash = _passwordCrypt.HashPassword(Guid.NewGuid().ToString("N"));

        Athlete athlete = new(email, fullName, passwordHash);
        athlete.SetUsername(await GenerateUniqueUsernameAsync(email));
        athlete.PromoteToInstructor();

        await _athleteRepository.AddAthleteAsync(athlete);
    }

    /// <summary>Updates the instructor's name, email and username.</summary>
    public async Task UpdateInstructorAsync(Guid id, UpdateInstructorDto dto)
    {
        // The repository update is shared with athletes, so adapt to its DTO (id comes from the route).
        UpdateAthleteDto athleteDto = new(id, dto.Email, dto.FirstName, dto.LastName, dto.Username);
        await _instructorRepository.UpdateInstructorAsync(athleteDto);
    }

    /// <summary>Demotes an instructor back to a regular member.</summary>
    public async Task DemoteInstructorAsync(Guid id) =>
        await _instructorRepository.DeleteInstructorAsync(id);

    // Derives a unique username from the email's local part (foo, foo1, foo2, …).
    private async Task<string> GenerateUniqueUsernameAsync(EmailAddress email)
    {
        string baseName = email.Address.Split('@')[0];
        string username = baseName;
        int count = 0;
        while (await _athleteRepository.GetAthleteByUsernameAsync(username) != null)
        {
            count++;
            username = $"{baseName}{count}";
        }
        return username;
    }
}
