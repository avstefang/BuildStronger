using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
using Application.Interface;
using Application.Template;
using Application.Dto;

namespace Application.Service;

public class RegisterAthleteService(IAthleteRepository athleteRepository, IPasswordCrypt passwordCrypt, IEmailSender? emailSender)
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
}