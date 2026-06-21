using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Domain.Entity;
using Domain.Value_object;

namespace Application.Interface;

public interface IAthleteRepository
{
    Task<Athlete> GetAthleteById(Guid id);
    Task<Athlete?> GetAthleteByEmailAsync(EmailAddress email);
    Task<IEnumerable<Athlete>> GetAllAthletesAsync();
    Task<Athlete?> GetAthleteByUsernameAsync(string username);
    Task<bool> IsAthleteUsernameTakenAsync(string username);
    Task AddAthleteAsync(Athlete athlete);
    Task UpdateAthleteAsync(Athlete athlete);
    Task DeleteAthleteByEmailAsync(EmailAddress email);
}