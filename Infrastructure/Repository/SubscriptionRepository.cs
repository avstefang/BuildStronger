using Domain.Entity;
using Application.Interface;
using Infrastructure;
using Domain.Enum;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class SubscriptionRepository(SubscriptionDbContext dbConnection) : Repository<Subscription, Guid>(dbConnection), ISubscriptionRepository
{
    public async Task AddSubscriptionAsync(Subscription subscription)
    {
        await AddAsync(subscription);
    }

    public async Task<IEnumerable<Subscription>?> GetAllSubscriptionsAsync()
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

    public async Task<IEnumerable<Subscription>?> GetSubscriptionsByAthleteIdAsync(Guid athleteId)
    {
        IEnumerable<Subscription>? subscriptions = await DbContext.Set<Subscription>()
            .Include(s => s.SubscriptionPlan)
            .Where(s => EF.Property<Guid>(s, "AthleteId") == athleteId)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync();

        return subscriptions;
    }

    public async Task<Subscription?> GetSubscriptionByAthleteIdAsync(Guid athleteId) =>
        await GetSubscriptionsByAthleteIdAsync(athleteId).ContinueWith(t => t.Result?.FirstOrDefault());

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
