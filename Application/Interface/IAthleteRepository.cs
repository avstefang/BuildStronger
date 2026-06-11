using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Domain.Entity;
using Domain.Value_object;

namespace Application.Interface;

public interface IAthleteRepository : IRepository<Athlete, Guid>
{
    Task<Athlete> GetAthleteById(Guid id);
    Task<Athlete> GetAthleteByEmailAsync(EmailAddress email);
    Task<IEnumerable<Athlete>> GetAllAthletesAsync();
    Task AddAthleteAsync(Athlete athlete);
    Task UpdateAthleteAsync(UpdateAthleteDto athleteDto);
    Task UpdateAthletePasswordAsync(UpdatePasswordDto athletePasswordDto);
    Task DeleteAthleteByEmailAsync(EmailAddress email);
}