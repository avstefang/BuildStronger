using Application.Interface;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class ActivateLatentSubscriptionService(ISubscriptionRepository subscriptionRepository)
{
    private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
    public bool ActivateSubscription(Subscription subscription)
    {
        if (subscription.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return false;
        }

        subscription.ActivateSubscription();
        _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription);
        return true;
    }
}