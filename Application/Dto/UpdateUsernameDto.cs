using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UpdateUsernameDto(string email, string username)
{
    public string Email { get; private set; } = email;
    public string Username { get; private set; } = username;
}