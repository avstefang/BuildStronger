using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class CancelSubscriptionService
{
    public void CancelSubscription(Athlete athlete)
    {
        var subscription = athlete.GetLastSubscription();
        subscription.CancelSubscription();
    }
}