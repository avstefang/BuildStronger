using Domain.Entity;
using Application.Interface;
using Infrastructure;
using Domain.Enum;
using Infrastructure.Context_model;

namespace Infrastructure.Repository;

public class SubscriptionRepository(SubscriptionDbContext dbConnection) : Repository<Subscription, Guid>(dbConnection), ISubscriptionRepository
{
    public async Task AddSubscriptionAsync(Subscription subscription)
    {
        await AddAsync(subscription);
    }

    public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
    {
        return await GetAllAsync();
    }

    public async Task<Subscription> GetSubscriptionByIdAsync(Guid subscriptionId)
    {
        var subscription = await GetByIdAsync(subscriptionId);
        if (subscription == null)
            throw new InvalidOperationException($"Subscription with ID {subscriptionId} not found");
        return subscription;
    }

    public async Task<SubscriptionStatus> UpdateSubscriptionStatusAsync(Subscription subscription)
    {
        await UpdateAsync(subscription);
        return subscription.Status;
    }

    public async Task DeleteSubscriptionAsync(Guid subscriptionId)
    {
        var subscription = await GetByIdAsync(subscriptionId);
        if (subscription != null)
        {
            DbContext.Set<Subscription>().Remove(subscription);
            await DbContext.SaveChangesAsync();
        }
    }
}
