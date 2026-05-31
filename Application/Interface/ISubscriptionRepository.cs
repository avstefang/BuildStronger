using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription> GetSubscriptionAsync(Subscription subscription);
    Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();
    Task AddSubscriptionAsync(Subscription subscription);
    Task UpdateSubscriptionAsync(Subscription subscription);
    Task DeleteSubscriptionAsync(Subscription subscription);
}
