using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UpdatePasswordDto(Guid id, string currentPassword, string newPassword)
{
    public Guid Id { get; private set; } = id;
    public string CurrentPassword { get; private set; } = currentPassword;
    public string NewPassword { get; private set; } = newPassword;
}