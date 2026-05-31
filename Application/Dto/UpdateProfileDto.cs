using Domain.Enum;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UpdateProfileDto
{
    public Guid Id { get; set; }
    public required EmailAddress EmailAddress { get; set; }
    public required FullName FullName { get; set; }
    public string Username { get; set; } = string.Empty;
}