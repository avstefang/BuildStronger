using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Domain.Entity;
using Domain.Enum;

namespace Application.Interface;

public interface ISubscriptionRepository
{
    Task<Subscription> GetSubscriptionByIdAsync(Guid subscriptionId);
    Task<IEnumerable<Subscription>?> GetAllSubscriptionsAsync();
    Task<IEnumerable<Subscription>?> GetSubscriptionsByAthleteIdAsync(Guid athleteId);
    Task<Subscription?> GetSubscriptionByAthleteIdAsync(Guid athleteId);
    Task AddSubscriptionAsync(Subscription subscription);
    Task<SubscriptionStatus> UpdateSubscriptionStatusAsync(Subscription subscription);
    Task DeleteSubscriptionAsync(Guid subscriptionId);
}
