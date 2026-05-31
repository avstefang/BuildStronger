using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interface;

public interface IEmailSender
{
    Task SendEmailAsync(EmailAddress email, string subject, string htmlMessage);
}