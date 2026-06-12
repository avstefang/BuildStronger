using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Application.Interface;
using Domain.Entity;
using Domain.Exception;
using Domain.Value_object;
using Infrastructure;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class AthleteRepository(AthleteDbContext dbConnection) : Repository<Athlete, Guid>(dbConnection), IAthleteRepository
{
    public async Task AddAthleteAsync(Athlete athlete)
    {
        await AddAsync(athlete);
    }

    public async Task DeleteAthleteByEmailAsync(EmailAddress email)
    {
        Athlete athlete = await GetAthleteByEmailAsync(email);
        if (athlete != null)
            await DeleteAsync(athlete.Id);
    }

    public async Task<IEnumerable<Athlete>> GetAllAthletesAsync()
    {
        return GetAllAsync().Result;
    }

    public async Task<Athlete> GetAthleteByEmailAsync(EmailAddress email)
    {
        return await DbContext.Set<Athlete>()
            .FirstOrDefaultAsync(a => a.EmailAddress.Address == email.Address)
            ?? throw new AthleteNotFoundException($"Athlete with email '{email.Address}' not found.");
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
        Athlete athlete = await GetAthleteById(athleteDto.Id);
        athlete.ChangeEmailAddress(athleteDto.EmailAddress);
        athlete.ChangeFullName(athleteDto.FullName);
        athlete.SetUsername(athleteDto.Username);
        await UpdateAsync(athlete);
    }

    public async Task UpdateAthletePasswordAsync(UpdatePasswordDto athletePasswordDto)
    {
        Athlete athlete = await GetAthleteById(athletePasswordDto.Id);
        athlete.SetPassword(athletePasswordDto.NewPassword);
        await UpdateAsync(athlete);
    }
}