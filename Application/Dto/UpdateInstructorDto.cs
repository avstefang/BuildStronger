using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class UpdateInstructorDto(string firstName, string lastName, string email, string username)
{
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public string Email { get; set; } = email;
    public string Username { get; set; } = username;
}