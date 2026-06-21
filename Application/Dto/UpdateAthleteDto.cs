using Domain.Entity;
using Domain.Enum;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UpdateAthleteDto(Guid athleteId, string emailAddress, string firstName, string lastName, string username)
{
    public Guid AthleteId { get; private set; } = athleteId;
    public string EmailAddress { get; private set; } = emailAddress;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public string Username { get; private set; } = username;
}