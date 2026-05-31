using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exception;

public class InstructorNotFoundException : System.Exception
{
    public string? InstructorName { get; }

    public InstructorNotFoundException(string message) : base(message) { }

    public InstructorNotFoundException(string message, string instructorName) : base(message)
    {
        InstructorName = instructorName;
    }
}