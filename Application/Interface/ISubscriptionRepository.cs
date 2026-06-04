using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
using Domain.Enum;

namespace Application.Interface;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription> RetrieveSubscriptionAsync(Subscription subscription);
    Task<IEnumerable<Subscription>> RetrieveAllSubscriptionsAsync();
    Task<SubscriptionStatus> RegisterSubscriptionAsync(Subscription subscription);
    Task<SubscriptionStatus> UpdateSubscriptionStatusAsync(Subscription subscription);
}
