using Application.Dto;
using Application.Interface;
using Application.Template;
using Domain.Entity;
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

    public async Task RegisterAthleteAsync(Athlete athlete)
    {
        // Hash the password before saving
        athlete.SetPassword(_passwordCrypt.HashPassword(athlete.Password));

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
        // Retrieve the athlete by email
        var athlete = await _athleteRepository.GetAthleteByEmailAsync(email);
        if (athlete == null)
        {
            return null;
        }

        // Verify the password
        string passwordString = _passwordCrypt.SecureToPlain(password);
        bool verifyPassword = _passwordCrypt.VerifyPassword(passwordString, athlete.Password);
        if (verifyPassword)
        {
            return athlete;
        }

        return null;
    }

    public async Task UpdateAthlete(UpdateAthleteDto updateAthleteDto)
    {
        Athlete athlete = await _athleteRepository.GetAthleteByEmailAsync(updateAthleteDto.EmailAddress);
        athlete.ChangeEmailAddress(updateAthleteDto.EmailAddress);
        athlete.ChangeFullName(updateAthleteDto.FullName);
        athlete.SetUsername(updateAthleteDto.Username);

        await _athleteRepository.UpdateAthleteAsync(athlete);
    }
}