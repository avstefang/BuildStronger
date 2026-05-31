using Application.Interface;
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
    public async Task<bool> LoginAthleteAsync(EmailAddress email, SecureString password)
    {
        // Retrieve the athlete by email
        var athlete = await _athleteRepository.GetAthleteByEmailAsync(email);
        if (athlete == null)
        {
            return false; // Athlete not found
        }
        // Verify the password
        string passwordString = _passwordCrypt.SecureToPlain(password);
        return _passwordCrypt.VerifyPassword(passwordString, athlete.Password);
    }
}