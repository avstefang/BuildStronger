using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exception;

public class SubscriptionStartDateNotPassedException : System.Exception
{
    public SubscriptionStartDateNotPassedException(string message) : base(message)
    {
    }
}