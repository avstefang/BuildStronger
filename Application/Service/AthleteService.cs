using Application.Dto;
using Application.Interface;
using Application.Template;
using Domain.Entity;
using Domain.Exception;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Application.Service;

public class AthleteService(IAthleteRepository athleteRepository, IPasswordCrypt passwordCrypt, IEmailSender? emailSender)
{
    private readonly IAthleteRepository _athleteRepository = athleteRepository;
    private readonly IPasswordCrypt _passwordCrypt = passwordCrypt;
    private readonly IEmailSender? _emailSender = emailSender;

    public async Task RegisterAthleteAsync(RegisterAthleteDto athleteDto)
    {
        string passwordHash = _passwordCrypt.HashPassword(athleteDto.Password);
        var email = new EmailAddress(athleteDto.Email);
        var fullName = new FullName(athleteDto.FirstName, athleteDto.LastName);
        Athlete athlete = new(email, fullName, passwordHash);

        // Save the athlete to the repository
        await _athleteRepository.AddAthleteAsync(athlete);

        // Send a welcome email if email sender is provided
        if (_emailSender != null)
        {
            EmailContentDto template = EmailTemplate.WelcomeEmail(athlete.FullName);
            await _emailSender.SendEmailAsync(athlete.EmailAddress, template.Subject, template.Body);
        }
    }

    public async Task<Athlete?> LoginAthleteAsync(EmailAddress email, SecureString password)
    {
        Athlete athlete;
        try
        {
            athlete = await _athleteRepository.GetAthleteByEmailAsync(email);
        }
        catch (AthleteNotFoundException)
        {
            return null;
        }

        string passwordString = _passwordCrypt.SecureToPlain(password);
        bool verifyPassword = _passwordCrypt.VerifyPassword(passwordString, athlete.Password);
        return verifyPassword ? athlete : null;
    }

    public async Task UpdateAthlete(UpdateAthleteDto updateAthleteDto)
    {
        await _athleteRepository.UpdateAthleteAsync(updateAthleteDto);
    }
}