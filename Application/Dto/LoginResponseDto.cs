using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class LoginResponseDto(string token, GetAthleteDto athlete)
{
    public string Token { get; init; } = token;
    public GetAthleteDto Athlete { get; init; } = athlete;
}