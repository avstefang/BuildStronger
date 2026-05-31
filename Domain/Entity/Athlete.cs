using Domain.Enum;
using Domain.Value_object;

namespace Domain.Entity;

public class Athlete(EmailAddress emailAddress, FullName fullName, string password)
{
    private readonly List<Subscription> _subscriptions = [];

    public Guid Id { get; private set; } = Guid.NewGuid();
    public required EmailAddress EmailAddress { get; set; } = emailAddress;
    public required FullName FullName { get; set; } = fullName;
    public string Password { get; private set; } = password;
    public Role Role { get; private set; } = Role.User;
    public string Username { get; private set; } = string.Empty;
    public PhotoPath? PhotoPath { get; set; } = null;
    public IReadOnlyList<Subscription> Subscriptions => _subscriptions;

    // Promote the Athlete to an Instructor role
    public void PromoteToInstructor()
    {
        Role = Role.Instructor;
    }

    // Demote the Athlete back to a User role
    public void DemoteToUser()
    {
        Role = Role.User;
    }

    // Test if the Athlete is also an Instructor
    public bool IsInstructor() => Role == Role.Instructor;

    public bool HasActiveSubscription()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Expire any subscriptions that have passed their end date
        foreach (var s in _subscriptions)
        {
            var endDate = s.StartDate.AddMonths(s.SubscriptionPlan.DurationInMonths);
            if (endDate < today)
                s.ExpireSubscription();
        }

        // Return true if any subscription is still active
        return _subscriptions.Any(s => s.Status == SubscriptionStatus.Active);
    }

    public void AddSubscription(Subscription subscription)
    {
        if (HasActiveSubscription())
            throw new InvalidOperationException("Athlete already has an active subscription.");

        _subscriptions.Add(subscription);
    }

    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        Password = password;
    }

    public void SetUsername(string username)
    {
        Username = username;
    }
}