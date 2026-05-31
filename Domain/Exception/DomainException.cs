using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exception;

public class DomainException : System.Exception
{
    public string Details { get; set; } = string.Empty;

    public DomainException(string message) : base(message) { }

    public DomainException(string message, string details) : base(message)
    {
        Details = details;
    }
}