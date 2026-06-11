using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface ISubscriptionPlanRepository : IRepository<SubscriptionPlan, Guid>
{
    Task<SubscriptionPlan> GetSubscriptionPlanById(Guid subscriptionPlanId);
    Task<SubscriptionPlan> GetSubscriptionPlanByNameAsync(string name);
    Task<IEnumerable<SubscriptionPlan>> GetAllSubscriptionPlansAsync();
    Task AddSubscriptionPlanAsync(SubscriptionPlan subscriptionPlan);
    Task UpdateSubscriptionPlanAsync(SubscriptionPlan subscriptionPlan);
    Task DeleteSubscriptionPlanAsync(Guid subscriptionPlanId);
}
