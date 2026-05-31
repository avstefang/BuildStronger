using System;

namespace Domain.Exception;

public class AthleteNotFoundException : System.Exception
{
    public string? AthleteName { get; } = null;

    public AthleteNotFoundException() { }

    public AthleteNotFoundException(string message) : base(message) { }

    public AthleteNotFoundException(string message, string athleteName) : base(message)
    {
        AthleteName = athleteName;
    }
}