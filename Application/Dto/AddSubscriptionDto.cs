using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class AddSubscriptionDto
{
    public string Email { get; private set; }
    public ProcessorId ProcessorId { get; private set; }
    public Guid SubscriptionPlanId { get; private set; }
    public DateOnly? StartDate { get; private set; }
    public bool AutoRenew { get; private set; }

    public AddSubscriptionDto(string email, ProcessorId processorId, Guid subscriptionPlanId, DateOnly? startDate = null, bool autoRenew = false)
    {
        if (null != startDate && startDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Start date cannot be in the past.", nameof(startDate));

        Email = email;
        ProcessorId = processorId;
        SubscriptionPlanId = subscriptionPlanId;
        StartDate = startDate;
        AutoRenew = autoRenew;
    }

    public void ChangeStartDate(DateOnly startDate)
    {
        if (null != StartDate && startDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Start date cannot be in the past.", nameof(startDate));

        StartDate = startDate;
    }
}