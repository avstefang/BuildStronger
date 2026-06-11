using Application.Interface;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repository;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(EmailAddress email, string subject, string htmlMessage)
    {
        Console.WriteLine("Sending email to: " + email.Address);
        return Task.CompletedTask;
    }
}