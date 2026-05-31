using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exception;

public class DurationNotValidException : System.Exception
{
    public int DurationInMinutes { get; } = 0;
    public DurationNotValidException() { }
    public DurationNotValidException(string message) : base(message) { }
    public DurationNotValidException(string message, int durationInMinutes) : base(message)
    {
        DurationInMinutes = durationInMinutes;
    }
}