using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class CreateInstructorDto(string firstName, string lastName, string email)
{
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public string Email { get; set; } = email;
}