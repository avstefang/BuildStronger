using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
using Domain.Value_object;

namespace Application.Interface;

public interface IAthleteRepository : IRepository<Athlete>
{
    Task<Athlete> GetAthleteByEmailAsync(EmailAddress email);
    Task<IEnumerable<Athlete>> GetAllAthletesAsync();
    Task AddAthleteAsync(Athlete athlete);
    Task UpdateAthleteAsync(Athlete athlete);
    Task DeleteAthleteByEmailAsync(EmailAddress email);
    Task AddSubscriptionAsync(Athlete athlete, Subscription subscription);
}
