using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Application.Interface;
using Domain.Entity;

namespace Application.Service;

class UpdateAthleteService(IAthleteRepository athleteRepository)
{
    private readonly IAthleteRepository _athleteRepository = athleteRepository;

    public async Task UpdateAthlete(UpdateAthleteDto updateAthleteDto)
    {
        Athlete athlete = await _athleteRepository.GetAthleteByEmailAsync(updateAthleteDto.EmailAddress);
        athlete.ChangeEmailAddress(updateAthleteDto.EmailAddress);
        athlete.ChangeFullName(updateAthleteDto.FullName);
        athlete.SetUsername(updateAthleteDto.Username);

        await _athleteRepository.UpdateAthleteAsync(athlete);
    }
}