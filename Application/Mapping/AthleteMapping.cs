using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class AthleteMapping
{
    public static GetAthleteDto ToDto(this Athlete athlete) => new()
    {
        Id = athlete.Id,
        Email = athlete.EmailAddress.Address,
        FirstName = athlete.FullName.FirstName,
        LastName = athlete.FullName.LastName,
        Username = athlete.Username,
        Role = athlete.Role.ToString()
    };

    public static GetAthleteSummaryDto ToSummaryDto(this Athlete athlete) => new()
    {
        Id = athlete.Id,
        FullName = athlete.FullName.ToString(),
        Email = athlete.EmailAddress.Address
    };
}
