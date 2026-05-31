using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IAthleteRepository : IRepository<Athlete>
{
    Task<Athlete> GetAthleteAsync(Athlete athlete);
    Task<IEnumerable<Athlete>> GetAllAthletesAsync();
    Task AddAthleteAsync(Athlete athlete);
    Task UpdateAthleteAsync(Athlete athlete);
    Task DeleteAthleteAsync(Athlete athlete);
}
