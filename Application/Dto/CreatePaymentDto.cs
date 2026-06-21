using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class CreatePaymentDto(string processorId, Guid subscriptionId)
{
    public string ProcessorId { get; set; } = processorId;
    public Guid SubscriptionId { get; set; } = subscriptionId;
}