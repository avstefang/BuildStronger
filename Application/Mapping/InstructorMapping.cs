using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class InstructorMapping
{
    public static GetInstructorDto ToDto(this Instructor instructor) => new()
    {
        Id = instructor.Athlete.Id,
        Firstname = instructor.Athlete.FullName.FirstName,
        Lastname = instructor.Athlete.FullName.LastName,
        Email = instructor.Athlete.EmailAddress.Address,
        Username = instructor.Athlete.Username,
        PhotoFile = instructor.Athlete.PhotoPath?.Path
    };
}