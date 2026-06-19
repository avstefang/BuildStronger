using Application.Dto;
using Application.Interface;
using Application.Mapping;
using Domain.Entity;
using Domain.Enum;
using Domain.Exception;
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

    public async Task<IEnumerable<GetSubscriptionDto>> GetAthleteSubscriptionsAsync(EmailAddress emailAddress)
    {
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(emailAddress) ??
            throw new AthleteNotFoundException($"Athlete with email '{emailAddress}' not found.");

        return athlete.Subscriptions.OrderByDescending(s => s.StartDate).Select(s => s.ToDto());
    }

    public async Task<GetAthleteDto> RegisterSubscriptionAsync(AddSubscriptionDto dto, string email)
    {
        EmailAddress? emailAddress = new(email);
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(emailAddress) ??
            throw new InvalidOperationException($"Athlete with email '{email}' not found.");

        SubscriptionPlan? subscriptionPlan = await _subscriptionPlanRepository.GetSubscriptionPlanByIdAsync(dto.SubscriptionPlanId) ??
            throw new InvalidOperationException($"Subscription plan with ID '{dto.SubscriptionPlanId}' not found.");

        PaymentMethod paymentMethod = Enum.Parse<PaymentMethod>(dto.PaymentMethod);

        Subscription? subscription = new(subscriptionPlan, dto.StartDate);
        ProcessorId inputProcessorId = new(dto.ProcessorId);
        Payment? payment = new(inputProcessorId, subscription, paymentMethod);

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

        await _subscriptionRepository.AddSubscriptionAsync(subscription, athlete.Id);
        await _paymentRepository.CreatePaymentAsync(payment);

        athlete.AddSubscription(subscription);
        return athlete.ToDto();
    }

    public async Task<GetAthleteDto> ChangeSubscriptionAsync(AddSubscriptionDto dto, string email)
    {
        EmailAddress? emailAddress = new(email);
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(emailAddress) ??
            throw new InvalidOperationException($"Athlete with email '{email}' not found.");

        Subscription? activeSubscription = athlete.GetActiveSubscription();
        DateOnly startDate = activeSubscription != null ? activeSubscription.GetEndDate().AddDays(1) : dto.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        dto.ChangeStartDate(startDate);
        return await RegisterSubscriptionAsync(dto, email);
    }

    public async Task CancelSubscriptionAsync(string email)
    {
        EmailAddress? emailAddress = new(email);
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(emailAddress) ??
            throw new InvalidOperationException($"Athlete with email '{email}' not found.");

        IEnumerable<Subscription>? subscriptions = await _subscriptionRepository.GetSubscriptionsByAthleteIdAsync(athlete.Id);
        Subscription subscription = subscriptions?
            .OrderByDescending(s => s.StartDate)
            .FirstOrDefault(s => s.Status == Domain.Enum.SubscriptionStatus.Active)
            ?? throw new InvalidOperationException($"No active subscription found for '{email}'.");

        subscription.CancelSubscription();
        await _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription);
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

    public async Task SetAutoRenewAsync(string email, bool enable)
    {
        EmailAddress emailAddress = new(email);
        Athlete athlete = await _athleteRepository.GetAthleteByEmailAsync(emailAddress) ??
            throw new InvalidOperationException($"Athlete with email '{email}' not found.");

        IEnumerable<Subscription>? subscriptions = await _subscriptionRepository.GetSubscriptionsByAthleteIdAsync(athlete.Id);
        Subscription subscription = subscriptions?.OrderByDescending(s => s.StartDate).FirstOrDefault() ??
            throw new InvalidOperationException($"No subscription found for '{email}'.");

        if (enable)
            subscription.EnableAutoRenewal();
        else
            subscription.DisableAutoRenewal();

        await _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription);
    }

    public async Task ActivateSubscriptionAsync(string email)
    {
        EmailAddress? emailAddress = new(email);
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(emailAddress) ??
            throw new InvalidOperationException($"Athlete with email '{email}' not found.");
        IEnumerable<Subscription>? subscriptions = await _subscriptionRepository.GetSubscriptionsByAthleteIdAsync(athlete.Id);
        Subscription? subscription = (subscriptions?.OrderByDescending(s => s.StartDate).FirstOrDefault()) ??
            throw new InvalidOperationException($"No active subscription found for '{email}'.");

        if (subscription.Status == SubscriptionStatus.Active)
            return;

        // Still in the future → can't activate yet. (Today or earlier is allowed.)
        if (subscription.Status == SubscriptionStatus.WaitingActivation && subscription.StartDate > DateOnly.FromDateTime(DateTime.Now))
            throw new SubscriptionStartDateNotPassedException($"Subscription for '{email}' cannot yet be activated.");

        subscription.ActivateSubscription();
        await _subscriptionRepository.UpdateSubscriptionStatusAsync(subscription);
    }
}