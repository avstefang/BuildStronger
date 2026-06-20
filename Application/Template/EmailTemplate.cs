using Application.Dto;
using Domain.Value_object;
using System.Globalization;

namespace Application.Template;

public class EmailTemplate
{
    private static readonly CultureInfo Dutch = CultureInfo.GetCultureInfo("nl-NL");

    // Wraps body content in a consistent, email-client-safe HTML layout (inline styles only,
    // since many clients strip <style> blocks). Keeps every mail on-brand and professional.
    private static string Wrap(string heading, string innerHtml) => $"""
        <!DOCTYPE html>
        <html lang="nl">
        <head>
            <meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        </head>
        <body style="margin:0;padding:0;background-color:#f4f5f7;font-family:'Segoe UI',Arial,sans-serif;">
            <div style="max-width:600px;margin:0 auto;padding:24px;">
                <div style="background:#1a1a2e;border-radius:12px 12px 0 0;padding:28px;text-align:center;">
                    <span style="color:#ffffff;font-size:22px;font-weight:600;letter-spacing:1px;">BUILD STRONGER</span>
                </div>
                <div style="background:#ffffff;padding:32px 28px;border-radius:0 0 12px 12px;color:#1a1a2e;font-size:15px;line-height:1.6;">
                    <h2 style="margin:0 0 16px;font-size:19px;color:#1a1a2e;">{heading}</h2>
                    {innerHtml}
                    <p style="margin:24px 0 0;color:#1a1a2e;">
                        Met krachtige groet,<br/>
                        <strong>Team Build Stronger</strong>
                    </p>
                </div>
                <p style="text-align:center;color:#9aa0a6;font-size:12px;margin:16px 0 0;">
                    © Build Stronger Sport Club · Deze e-mail is automatisch verzonden, beantwoorden is niet nodig.
                </p>
            </div>
        </body>
        </html>
        """;

    // A small highlighted details panel, used for class/booking specifics.
    private static string DetailsBox(string innerHtml) =>
        $"""<div style="background:#f4f5f7;border-radius:8px;padding:16px 18px;margin:16px 0;">{innerHtml}</div>""";

    public static EmailContentDto WelcomeEmail(FullName fullName) => new(
        "Welkom bij Build Stronger",
        Wrap("Welkom bij Build Stronger!", $"""
            <p>Beste {fullName},</p>
            <p>We zijn blij je te mogen verwelkomen bij onze community. Je account staat klaar — tijd om sterker te worden!</p>
            <p>Heb je nog geen abonnement? Open de app om er een af te sluiten en je eerste les te boeken.</p>
        """));

    public static EmailContentDto SubscriptionSucceededEmail(FullName fullName) => new(
        "Je abonnement is actief",
        Wrap("Betaling geslaagd", $"""
            <p>Beste {fullName},</p>
            <p>Bedankt! Je betaling is gelukt en je abonnement is geactiveerd. Je kunt nu lessen reserveren via de app.</p>
            <p>We zien je graag in de sportschool.</p>
        """));

    public static EmailContentDto SubscriptionFailedEmail(FullName fullName) => new(
        "Je abonnement kon niet worden aangemaakt",
        Wrap("Betaling mislukt", $"""
            <p>Beste {fullName},</p>
            <p>Helaas is je betaling niet gelukt, waardoor we je abonnement niet konden aanmaken.</p>
            <p>Controleer je betaalgegevens en probeer het opnieuw via de app. Lukt het nog steeds niet? Neem dan contact met ons op.</p>
        """));

    public static EmailContentDto SubscriptionExpiredEmail(FullName fullName) => new(
        "Je abonnement is verlopen",
        Wrap("Abonnement verlopen", $"""
            <p>Beste {fullName},</p>
            <p>Je abonnement is verlopen. Je kunt op dit moment geen lessen meer reserveren.</p>
            <p>Wil je doorgaan met trainen? Verleng of kies een nieuw abonnement in de app — je bent zo weer van de partij.</p>
        """));

    public static EmailContentDto WaitlistAcceptedEmail(FullName fullName, string className, DateTime start, string room) => new(
        $"Je hebt een plek bij {className}!",
        Wrap("Je staat niet langer op de wachtlijst", $"""
            <p>Beste {fullName},</p>
            <p>Goed nieuws! Er is een plek vrijgekomen en je reservering is bevestigd:</p>
            {DetailsBox($"""
                <p style="margin:0;"><strong>{className}</strong></p>
                <p style="margin:4px 0 0;">{start.ToString("dddd d MMMM yyyy", Dutch)} om {start.ToString("HH:mm", Dutch)}</p>
                <p style="margin:4px 0 0;color:#5f6368;">{room}</p>
            """)}
            <p>Niet aanwezig? Annuleer je reservering tijdig in de app, zodat iemand anders jouw plek kan overnemen.</p>
        """));
}
