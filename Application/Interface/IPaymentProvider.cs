using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interface;

public interface IPaymentProvider
{
    Task<string> CreatePaymentAsync(decimal amount, string currency, string description);
    Task<bool> VerifyPaymentAsync(string paymentId);
}