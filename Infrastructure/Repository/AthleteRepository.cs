using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Application.Interface;
using Domain.Entity;
using Domain.Exception;
using Domain.Repository;
using Domain.Value_object;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class AthleteRepository(AthleteDbContext dbConnection) : Repository<Athlete, Guid>(dbConnection), IAthleteRepository
{
    public async Task AddAthleteAsync(Athlete athlete)
    {
        Athlete? registeredAthlete = await GetAthleteByEmailAsync(athlete.EmailAddress);
        if (registeredAthlete != null)
        {
            throw new AthleteAlreadyExistsException($"Athlete with email '{athlete.EmailAddress.Address}' already exists.");
        }

        await AddAsync(athlete);
    }

    public async Task DeleteAthleteByEmailAsync(EmailAddress email)
    {
        Athlete? athlete = await GetAthleteByEmailAsync(email);
        if (athlete != null)
            await DeleteAsync(athlete.Id);
    }

    public async Task<IEnumerable<Athlete>> GetAllAthletesAsync()
    {
        return await DbContext.Set<Athlete>()
            .Include(a => a.Subscriptions)
            .ThenInclude(s => s.SubscriptionPlan)
            .ToListAsync();
    }

    public async Task<Athlete?> GetAthleteByEmailAsync(EmailAddress email)
    {
        return await DbContext.Set<Athlete>()
            .FirstOrDefaultAsync(a => a.EmailAddress.Address == email.Address)
            ?? null;
    }

    public async Task<Athlete?> GetAthleteByUsernameAsync(string username)
    {
        return await DbContext.Set<Athlete>()
            .FirstOrDefaultAsync(a => a.Username == username)
            ?? null;
    }

    public async Task<bool> IsAthleteUsernameTakenAsync(string username)
    {
        return await GetAthleteByUsernameAsync(username) != null;
    }

    public async Task<Athlete> GetAthleteById(Guid id)
    {
        Task<Athlete?> task = GetByIdAsync(id);
        return await task ?? throw new InvalidOperationException("Athlete not found");
    }

    public async Task UpdateAthleteAsync(Athlete athlete)
    {
        await UpdateAsync(athlete);
    }

    public async Task UpdateAthleteAsync(UpdateAthleteDto athleteDto)
    {
        Athlete athlete = await GetAthleteByEmailAsync(athleteDto.EmailAddress);
        athlete.ChangeEmailAddress(athleteDto.EmailAddress);
        athlete.ChangeFullName(athleteDto.FullName);
        athlete.SetUsername(athleteDto.Username);
        await UpdateAsync(athlete);
    }
}