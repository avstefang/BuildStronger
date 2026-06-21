using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UpdatePasswordDto
{
    public string Email { get; private set; }
    public string CurrentPassword { get; private set; }
    public string NewPassword { get; private set; }

    public UpdatePasswordDto(string email, string currentPassword, string newPassword)
    {
        Email = email;
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }
}