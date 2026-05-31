using Application.Dto;
using Domain.Enum;
using Domain.Exception;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Template;

public class EmailTemplate
{
    private static EmailContentDto GetEmailContent(FullName fullName, EmailType emailType, string? url = null)
    {
        return emailType switch
        {
            EmailType.WelcomeEmail => new EmailContentDto(
                "Welkom bij Build Stronger",
                Body: $"""
                    Beste {fullName},

                    <p>
                    We zijn blij je te mogen verwelkomen bij onze community!
                    </p>

                    <p>
                    Als je nog geen abonnement hebt afgesloten,
                    klik dan <a href="{url}">hier</a>.
                    </p>

                    <p>
                    Met krachtige groet,<br/>
                    Team Build Stronger
                    </p>
                """
            ),
            EmailType.PasswordResetEmail => new EmailContentDto("Verzoek wachtwoord resetten", $"Beste {fullName},\n\n"),
            EmailType.SubscriptionSucceededEmail => new EmailContentDto("Abonnement betaald", $"Beste {fullName},\n\n"),
            EmailType.SubscriptionExpiredEmail => new EmailContentDto("Abonnement verlopen", $"Beste {fullName},\n\n"),
            EmailType.SubscriptionFailedEmail => new EmailContentDto("Abonnement aanvragen mislukt", $"Beste {fullName},\n\n"),
            _ => throw new EmailNotValidException($"Invalid email type. Choose from {string.Join(", ", Enum.GetValues<EmailType>())}")
        };
    }

    public static EmailContentDto WelcomeEmail(FullName fullName) => (EmailContentDto)GetEmailContent(fullName, EmailType.WelcomeEmail, "_blank");

    public static EmailContentDto PasswordResetEmail(FullName fullName) => (EmailContentDto)GetEmailContent(fullName, EmailType.PasswordResetEmail);

    public static EmailContentDto SubscriptionSucceededEmail(FullName fullName) => (EmailContentDto)GetEmailContent(fullName, EmailType.SubscriptionSucceededEmail);

    public static EmailContentDto SubscriptionExpiredEmail(FullName fullName) => (EmailContentDto)GetEmailContent(fullName, EmailType.SubscriptionExpiredEmail);

    public static EmailContentDto SubscriptionFailedEmail(FullName fullName) => (EmailContentDto)GetEmailContent(fullName, EmailType.SubscriptionFailedEmail);
}