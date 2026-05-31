using Domain.Enum;
using Domain.Exception;
using System;

namespace Domain.Entity;

public class Subscription(SubscriptionPlan subscriptionPlan, DateOnly? startDate)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public SubscriptionPlan SubscriptionPlan { get; private set; } = subscriptionPlan ?? throw new ArgumentNullException(nameof(subscriptionPlan));
    public DateOnly StartDate { get; private set; } = startDate ?? DateOnly.FromDateTime(DateTime.Now);
    public bool AutoRenew { get; private set; } = false;
    public SubscriptionStatus Status { get; private set; }

    public void EnableAutoRenewal()
    {
        AutoRenew = true;
    }

    public void DisableAutoRenewal()
    {
        AutoRenew = false;
    }

    public void CancelSubscription()
    {
        Status = SubscriptionStatus.Cancelled;
    }

    public void ActivateSubscription()
    {
        if (Status == SubscriptionStatus.Expired || Status == SubscriptionStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot reactivate an expired or cancelled subscription.");
        }

        Status = SubscriptionStatus.Active;
    }

    public void ExpireSubscription()
    {
        Status = SubscriptionStatus.Expired;
    }

    public void AddSubscriptionPlan(SubscriptionPlan subscriptionPlan)
    {
        if (null != SubscriptionPlan)
            throw new InvalidOperationException("Subscription plan already set and cannot be changed.");

        SubscriptionPlan = subscriptionPlan;
    }
}
