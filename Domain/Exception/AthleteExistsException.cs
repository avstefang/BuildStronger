namespace Domain.Exception;

public class AthleteExistsException : System.Exception
{
    public string? AthleteName { get; } = null;
    public AthleteExistsException() { }
    public AthleteExistsException(string message) : base(message) { }
    public AthleteExistsException(string message, string athleteName) : base(message)
    {
        AthleteName = athleteName;
    }
}