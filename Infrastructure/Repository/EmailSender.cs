using Application.Interface;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MailKit;

namespace Infrastructure.Repository;

public class EmailSender(IConfiguration configuration) : IEmailSender
{
    private async Task<SmtpClient> SmtpConfiguration()
    {
        SmtpClient smtpClient = new();

        string smtpServer = configuration["Email:SmtpServer"] ??
            throw new MessageNotFoundException("SMTP server not found");
        int port = int.Parse(configuration["Email:SmtpPort"] ?? throw new InvalidOperationException("SMTP port not found"));
        string username = configuration["Email:Username"] ??
            throw new InvalidOperationException("SMTP username not found");
        string password = configuration["Email:Password"] ??
            throw new InvalidOperationException("SMTP password not found");
        await smtpClient.ConnectAsync(smtpServer, port, true);
        await smtpClient.AuthenticateAsync(username, password);
        return smtpClient;
    }

    private MimeMessage CreateEmailMessage(FullName fullName, EmailAddress email, string subject, string htmlMessage)
    {
        MimeMessage message = new()
        {
            Subject = subject,
            Body = new TextPart("html") { Text = htmlMessage }
        };

        string fromName = configuration["Email:FromName"] ??
            throw new InvalidOperationException("From name not found");
        string fromAddress = configuration["Email:FromEmail"] ??
            throw new InvalidOperationException("From address not found");
        message.From.Add(new MailboxAddress(fromName, fromAddress));
        return message;
    }

    public async Task SendEmailAsync(FullName fullName, EmailAddress email, string subject, string htmlMessage)
    {
        MimeMessage message = CreateEmailMessage(fullName, email, subject, htmlMessage);
        message.To.Add(new MailboxAddress(fullName.ToString(), email.Address));

        using SmtpClient smtpClient = await SmtpConfiguration();
        await smtpClient.SendAsync(message);
        await smtpClient.DisconnectAsync(true);
    }
}