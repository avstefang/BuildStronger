using Application.Interface;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class SubscriptionService(
    ISubscriptionPlanRepository subscriptionPlanRepository,
    ISubscriptionRepository subscriptionRepository,
    IAthleteRepository athleteRepository,
    IPaymentRepository paymentRepository,
    IPaymentProcessor paymentProcessor
)
{
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository = subscriptionPlanRepository;
    private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
    private readonly IAthleteRepository _athleteRepository = athleteRepository;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IPaymentProcessor _paymentProcessor = paymentProcessor;

    public async Task<Athlete> RegisterSubscriptionAsync(Athlete athlete, string subscriptionName, DateOnly? startDate, bool autoRenew = false)
    {
        SubscriptionPlan subscriptionPlan = await _subscriptionPlanRepository.GetSubscriptionPlanByNameAsync(subscriptionName);
        Subscription subscription = new(subscriptionPlan, startDate);
        Payment payment = new(subscription);

        Guid paymentId = await _paymentProcessor.ProcessPaymentAsync(payment);
        if (paymentId != Guid.Empty)
        {
            payment.SetProcessorId(paymentId);
            payment.IsSuccessful();

            if (autoRenew)
            {
                subscription.EnableAutoRenewal();
            }

            if (startDate == DateOnly.FromDateTime(DateTime.UtcNow))
            {
                subscription.ActivateSubscription();
            }
        }
        else
        {
            payment.IsFailed();
            subscription.FailSubscription();
        }

        await _paymentRepository.CreatePaymentAsync(payment);

        athlete.AddSubscription(subscription);
        await _subscriptionRepository.AddSubscriptionAsync(subscription);
        await _athleteRepository.UpdateAthleteAsync(athlete);
        return athlete;
    }

    public async Task<Athlete> ChangeSubscriptionAsync(Athlete athlete, string newSubscriptionName, DateOnly? startDate, bool autoRenew = false)
    {
        Subscription? lastSubscription = athlete.GetLastSubscription();
        if (newSubscriptionName == lastSubscription?.SubscriptionPlan.Name)
        {
            throw new InvalidOperationException("Athlete already has the specified subscription.");
        }

        DateOnly? endDateLastSubscription = lastSubscription?.GetEndDate() ?? null;
        if (startDate != null && startDate < endDateLastSubscription)
        {
            startDate = endDateLastSubscription.Value.AddDays(1);
        }

        SubscriptionPlan subscriptionPlan = await _subscriptionPlanRepository.GetSubscriptionPlanByNameAsync(newSubscriptionName);
        Subscription subscription = new(subscriptionPlan, startDate);
        Payment payment = new(subscription);

        Guid paymentId = await _paymentProcessor.ProcessPaymentAsync(payment);
        if (paymentId != Guid.Empty)
        {
            payment.SetProcessorId(paymentId);
            payment.IsSuccessful();

            if (autoRenew)
            {
                subscription.EnableAutoRenewal();
            }

            if (startDate == DateOnly.FromDateTime(DateTime.UtcNow))
            {
                subscription.ActivateSubscription();
            }
        }
        else
        {
            payment.IsFailed();
            subscription.FailSubscription();
        }

        await _paymentRepository.CreatePaymentAsync(payment);

        athlete.AddSubscription(subscription);
        await _subscriptionRepository.AddSubscriptionAsync(subscription);
        await _athleteRepository.UpdateAthleteAsync(athlete);
        return athlete;
    }

    public async Task CancelSubscriptionAsync(Athlete athlete)
    {
        var subscription = athlete.GetLastSubscription();
        if (subscription != null)
        {
            subscription.CancelSubscription();
            await _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription);
            await _athleteRepository.UpdateAthleteAsync(athlete);
        }
    }

    public async Task<bool> ActivateLatentSubscriptionAsync(Subscription subscription)
    {
        if (subscription.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return false;
        }

        subscription.ActivateSubscription();
        await _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription);
        return true;
    }
}