using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UpdatePasswordDto
{
    public Guid Id { get; set; }
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}