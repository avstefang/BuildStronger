using Domain.Enum;
using Domain.Value_object;

namespace Domain.Entity;

public class Athlete
{
    private readonly List<Subscription> _subscriptions = [];

    public Guid Id { get; private set; } = Guid.NewGuid();
    public EmailAddress EmailAddress { get; private set; }
    public FullName FullName { get; private set; }
    public string Password { get; private set; }
    public Role Role { get; private set; } = Role.User;
    public string Username { get; private set; } = string.Empty;
    public PhotoPath? PhotoPath { get; private set; } = null;
    public IReadOnlyList<Subscription> Subscriptions => _subscriptions;

    protected Athlete()
    {
        EmailAddress = null!;
        FullName = null!;
        Password = null!;
    }

    public Athlete(EmailAddress emailAddress, FullName fullName, string password)
    {
        EmailAddress = emailAddress;
        FullName = fullName;
        Password = password;
    }

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

    public void LoadSubscriptions(IEnumerable<Subscription> subscriptions)
    {
        if (null != Subscriptions)
        {
            throw new System.Exception("Loading subscriptions is only allowed during athlete creation. Use AddSubscription for adding new subscriptions.");
        }

        _subscriptions.AddRange(subscriptions);
    }

    public Subscription? GetActiveSubscription() =>
        _subscriptions.FirstOrDefault(s => s.GrantsAccessOn(DateOnly.FromDateTime(DateTime.UtcNow)));

    public Subscription? GetLastSubscription() =>
        _subscriptions.OrderByDescending(s => s.StartDate).FirstOrDefault() ?? null;

    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        Password = password;
    }

    public void SetUsername(string username)
    {
        if (Username == username)
            throw new ArgumentException("New username must be different from the current one.", nameof(username));

        Username = username;
    }

    public void ChangeEmailAddress(EmailAddress newEmailAddress)
    {
        if (EmailAddress == newEmailAddress)
            throw new ArgumentException("New email address must be different from the current one.", nameof(newEmailAddress));

        EmailAddress = newEmailAddress;
    }

    public void ChangeFullName(FullName newFullName)
    {
        if (FullName == newFullName)
            throw new ArgumentException("New full name must be different from the current one.", nameof(newFullName));

        FullName = newFullName;
    }

    public void ChangePhotoPath(PhotoPath? newPhotoPath)
    {
        if (PhotoPath == newPhotoPath)
            throw new ArgumentException("New photo path must be different from the current one.", nameof(newPhotoPath));
        PhotoPath = newPhotoPath;
    }
}