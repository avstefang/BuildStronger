using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exception;

public class SubscriptionException : System.Exception
{
    public string SubscriptionName { get; private set; } = string.Empty;
    public SubscriptionException(string message) : base(message) { }
    public SubscriptionException(string message, string subscriptionName) : base(message) {
        SubscriptionName = subscriptionName;
    }
}