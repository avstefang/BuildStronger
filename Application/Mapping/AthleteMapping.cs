using Application.Dto;
using Domain.Entity;

namespace Application.Mapping;

public static class AthleteMapping
{
    public static GetAthleteDto ToDto(this Athlete athlete)
    {
        // The current membership: the active (or cancelled-but-still-valid) subscription, else the most
        // recent one. Null when the athlete never subscribed, or when subscriptions weren't loaded.
        Subscription? subscription = athlete.GetActiveSubscription() ?? athlete.GetLastSubscription();

        return new()
        {
            Id = athlete.Id,
            Email = athlete.EmailAddress.Address,
            FirstName = athlete.FullName.FirstName,
            LastName = athlete.FullName.LastName,
            Username = athlete.Username,
            Role = athlete.Role.ToString(),
            Subscription = subscription?.ToDto()
        };
    }

    public static GetAthleteSummaryDto ToSummaryDto(this Athlete athlete) => new()
    {
        Id = athlete.Id,
        FullName = athlete.FullName.ToString(),
        Email = athlete.EmailAddress.Address
    };
}
