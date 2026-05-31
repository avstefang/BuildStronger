using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interface;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}