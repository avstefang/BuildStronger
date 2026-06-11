using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UsernameDto(Guid id, string username)
{
    public Guid Id { get; private set; } = id;
    public string Username { get; private set; } = username;
}