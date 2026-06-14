using Application.Dto;
using Application.Interface;
using Domain.Entity;
using Domain.Value_object;
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

    public async Task<Athlete> RegisterSubscriptionAsync(AddSubscriptionDto dto)
    {
        EmailAddress? emailAddress = new(dto.Email);
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(emailAddress) ??
            throw new InvalidOperationException($"Athlete with email '{dto.Email}' not found.");

        SubscriptionPlan? subscriptionPlan = await _subscriptionPlanRepository.GetSubscriptionPlanByIdAsync(dto.SubscriptionPlanId) ??
            throw new InvalidOperationException($"Subscription plan with ID '{dto.SubscriptionPlanId}' not found.");

        Subscription? subscription = new(subscriptionPlan, dto.StartDate);
        Payment? payment = new(dto.ProcessorId, subscription);

        ProcessorId? processorId = await _paymentProcessor.ProcessPaymentAsync(payment);
        if (processorId != null)
        {
            payment.IsSuccessful();

            if (dto.AutoRenew) subscription.EnableAutoRenewal();

            if (dto.StartDate == DateOnly.FromDateTime(DateTime.UtcNow))
                subscription.ActivateSubscription();
        }
        else
        {
            payment.IsFailed();
            subscription.FailSubscription();
        }

        await _paymentRepository.CreatePaymentAsync(payment);
        await _subscriptionRepository.AddSubscriptionAsync(subscription);

        athlete.AddSubscription(subscription);
        await _athleteRepository.UpdateAthleteAsync(athlete);
        return athlete;
    }

    public async Task<Athlete> ChangeSubscriptionAsync(AddSubscriptionDto dto)
    {
        EmailAddress? emailAddress = new(dto.Email);
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(emailAddress) ??
            throw new InvalidOperationException($"Athlete with email '{dto.Email}' not found.");

        Subscription? activeSubscription = athlete.GetActiveSubscription();
        DateOnly startDate = activeSubscription != null ? activeSubscription.GetEndDate().AddDays(1) : dto.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        dto.ChangeStartDate(startDate);
        return await RegisterSubscriptionAsync(dto);
    }

    public async Task CancelSubscriptionAsync(string email)
    {
        EmailAddress? emailAddress = new(email);
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(emailAddress) ??
            throw new InvalidOperationException($"Athlete with email '{email}' not found.");

        var subscription = athlete.GetLastSubscription();
        if (subscription != null)
        {
            subscription.CancelSubscription();
            await _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription);
            await _athleteRepository.UpdateAthleteAsync(athlete);
        }
    }

    public async Task ActivateLatentSubscriptionAsync()
    {
        IEnumerable<Subscription>? latentSubscriptions = await _subscriptionRepository.GetAllLatentSubscriptionsAsync();
        foreach (Subscription subscription in latentSubscriptions!)
        {
            subscription.ActivateSubscription();
            await _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription);
        }
    }
}