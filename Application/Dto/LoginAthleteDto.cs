using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class LoginAthleteDto(string email, string password)
{
    public string Email { get; private set; } = email;
    public string Password { get; private set; } = password;
}