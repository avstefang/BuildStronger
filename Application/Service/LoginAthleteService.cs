using Application.Interface;
using Domain.Entity;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Application.Service;

public class LoginAthleteService(IAthleteRepository athleteRepository, IPasswordCrypt passwordCrypt)
{
    private readonly IAthleteRepository _athleteRepository = athleteRepository;
    private readonly IPasswordCrypt _passwordCrypt = passwordCrypt;
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
}