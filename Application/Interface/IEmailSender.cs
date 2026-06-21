using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interface;

public interface IEmailSender
{
    Task SendEmailAsync(FullName fullName, EmailAddress email, string subject, string htmlMessage);
}