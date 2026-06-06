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

    public Athlete RegisterSubscription(Athlete athlete, string subscriptionName, DateOnly? startDate, bool autoRenew = false)
    {
        SubscriptionPlan subscriptionPlan = _subscriptionPlanRepository.RetrieveSubscriptionPlanByNameAsync(subscriptionName).Result;
        Subscription subscription = new(subscriptionPlan, startDate);
        Payment payment = new(subscription);

        Guid paymentId = _paymentProcessor.ProcessPaymentAsync(payment).Result;
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
                payment.Subscription.ActivateSubscription();
            }
        }
        else
        {
            payment.IsFailed();
            payment.Subscription.FailSubscription();
        }

        _paymentRepository.CreatePaymentAsync(payment);

        athlete.AddSubscription(subscription);
        _athleteRepository.AddSubscriptionAsync(athlete, subscription);
        return athlete;
    }

    public Athlete ChangeSubscription(Athlete athlete, string newSubscriptionName, DateOnly? startDate, bool autoRenew = false)
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

        SubscriptionPlan subscriptionPlan = _subscriptionPlanRepository.RetrieveSubscriptionPlanByNameAsync(newSubscriptionName).Result;
        Subscription subscription = new(subscriptionPlan, startDate);
        Payment payment = new(subscription);

        Guid paymentId = _paymentProcessor.ProcessPaymentAsync(payment).Result;
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
                payment.Subscription.ActivateSubscription();
            }
        }
        else
        {
            payment.IsFailed();
            payment.Subscription.FailSubscription();
        }

        _paymentRepository.CreatePaymentAsync(payment);

        athlete.AddSubscription(subscription);
        _athleteRepository.AddSubscriptionAsync(athlete, subscription);
        return athlete;
    }

    public void CancelSubscription(Athlete athlete)
    {
        var subscription = athlete.GetLastSubscription();
        subscription?.CancelSubscription();
    }

    public bool ActivateLatentSubscription(Subscription subscription)
    {
        if (subscription.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return false;
        }

        subscription.ActivateSubscription();
        _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription);
        return true;
    }
}