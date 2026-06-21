using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class SubscriptionDto(Guid athleteId, Subscription subscription)
{
    public Guid AthleteId { get; private set; } = athleteId;
    public Subscription Subscription { get; private set; } = subscription;
}