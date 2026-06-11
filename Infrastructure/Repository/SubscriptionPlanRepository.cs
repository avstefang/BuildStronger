using Domain.Entity;
using Application.Interface;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Context_model;

namespace Infrastructure.Repository;

public class SubscriptionPlanRepository(SubscriptionPlanDbContext dbConnection) : Repository<SubscriptionPlan, Guid>(dbConnection), ISubscriptionPlanRepository
{
    public async Task<SubscriptionPlan> GetSubscriptionPlanById(Guid subscriptionPlanId)
    {
        var plan = await GetByIdAsync(subscriptionPlanId);
        if (plan == null)
            throw new InvalidOperationException($"Subscription Plan with ID {subscriptionPlanId} not found");
        return plan;
    }

    public async Task<SubscriptionPlan> GetSubscriptionPlanByNameAsync(string name)
    {
        var plan = await DbContext.Set<SubscriptionPlan>().FirstOrDefaultAsync(p => p.Name == name);
        if (plan == null)
            throw new InvalidOperationException($"Subscription Plan with name '{name}' not found");
        return plan;
    }

    public async Task<IEnumerable<SubscriptionPlan>> GetAllSubscriptionPlansAsync()
    {
        return await GetAllAsync();
    }

    public async Task AddSubscriptionPlanAsync(SubscriptionPlan subscriptionPlan)
    {
        await AddAsync(subscriptionPlan);
    }

    public async Task UpdateSubscriptionPlanAsync(SubscriptionPlan subscriptionPlan)
    {
        await UpdateAsync(subscriptionPlan);
    }

    public async Task DeleteSubscriptionPlanAsync(Guid subscriptionPlanId)
    {
        await DeleteAsync(subscriptionPlanId);
    }
}
