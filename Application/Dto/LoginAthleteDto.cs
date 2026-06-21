using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class LoginAthleteDto(string usernameOrEmail, string password)
{
    public string UsernameOrEmail { get; private set; } = usernameOrEmail;
    public string Password { get; private set; } = password;
}