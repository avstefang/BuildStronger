using Application.Interface;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class RegisterSubscriptionService(
    ISubscriptionPlanRepository subscriptionPlanRepository,
    IAthleteRepository athleteRepository,
    IPaymentRepository paymentRepository,
    IPaymentProcessor paymentProcessor
) {
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository = subscriptionPlanRepository;
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
}