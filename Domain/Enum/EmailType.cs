using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enum;

public enum EmailType
{
    WelcomeEmail,
    PasswordResetEmail,
    SubscriptionSucceededEmail,
    SubscriptionExpiredEmail,
    SubscriptionFailedEmail
}