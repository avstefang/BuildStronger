using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface ISubscriptionPlanRepository : IRepository<SubscriptionPlan>
{
    Task<SubscriptionPlan> GetSubscriptionPlanAsync(SubscriptionPlan plan);
    Task<IEnumerable<SubscriptionPlan>> GetAllSubscriptionPlansAsync();
    Task AddSubscriptionPlanAsync(SubscriptionPlan plan);
    Task UpdateSubscriptionPlanAsync(SubscriptionPlan plan);
    Task DeleteSubscriptionPlanAsync(SubscriptionPlan plan);
}
