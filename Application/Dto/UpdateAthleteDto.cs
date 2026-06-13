using Domain.Entity;
using Domain.Enum;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UpdateAthleteDto(EmailAddress emailAddress, FullName fullName, string username)
{
    public EmailAddress EmailAddress { get; private set; } = emailAddress;
    public FullName FullName { get; private set; } = fullName;
    public string Username { get; private set; } = username;
}