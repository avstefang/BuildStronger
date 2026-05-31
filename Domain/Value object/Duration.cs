using Domain.Exception;

namespace Domain.Value_object;

public sealed record class Duration
{
    public int Minutes { get; }

    public Duration(int minutes)
    {
        if (minutes < 15 || minutes > 120)
        {
            throw new DomainException("A lesson must be between 15 minutes and two hours.", minutes.ToString());
        }

        Minutes = minutes;
    }
}
