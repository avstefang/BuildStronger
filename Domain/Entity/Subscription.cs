using Domain.Enum;
using Domain.Exception;
using System;

namespace Domain.Entity;

public class Subscription
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public SubscriptionPlan SubscriptionPlan { get; private set; }
    public DateOnly StartDate { get; private set; }
    public bool AutoRenew { get; private set; } = false;
    public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.WaitingActivation;

    protected Subscription()
    {
        SubscriptionPlan = null!;
        StartDate = DateOnly.FromDateTime(DateTime.Now);
    }

    public Subscription(SubscriptionPlan subscriptionPlan, DateOnly? startDate)
    {
        SubscriptionPlan = subscriptionPlan;
        StartDate = startDate ?? DateOnly.FromDateTime(DateTime.Now);
    }

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
        if (Status != SubscriptionStatus.Active)
        {
            throw new SubscriptionException("Only active subscriptions can be cancelled.", SubscriptionPlan.Name);
        }

        Status = SubscriptionStatus.Cancelled;
    }

    public void ActivateSubscription()
    {
        if (Status == SubscriptionStatus.Expired || Status == SubscriptionStatus.Cancelled || Status == SubscriptionStatus.Failed)
        {
            throw new InvalidOperationException("Cannot reactivate an expired, failed, or cancelled subscription.");
        }

        if (StartDate >= DateOnly.FromDateTime(DateTime.Now))
        {
            throw new InvalidOperationException("Cannot activate a subscription before its start date.");
        }

        Status = SubscriptionStatus.Active;
    }

    public void ExpireSubscription()
    {
        if (Status != SubscriptionStatus.Active)
        {
            throw new SubscriptionException("Only active subscriptions can be expired.", SubscriptionPlan.Name);
        }

        Status = SubscriptionStatus.Expired;
    }

    public void FailSubscription()
    {
        if (Status != SubscriptionStatus.WaitingActivation)
        {
            throw new SubscriptionException("Only subscriptions in 'WaitingActivation' status can be marked as failed.", SubscriptionPlan.Name);
        }

        Status = SubscriptionStatus.Failed;
    }

    public DateOnly GetEndDate() => StartDate.AddMonths(SubscriptionPlan.DurationInMonths);
}
