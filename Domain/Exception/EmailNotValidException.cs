using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exception;

public class EmailNotValidException : System.Exception
{
    public string? Email { get; } = null;
    public EmailNotValidException() { }
    public EmailNotValidException(string message) : base(message) { }
    public EmailNotValidException(string message, string email) : base(message)
    {
        Email = email;
    }
}