using Application.Dto;
using Application.Interface;
using Domain.Entity;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class PaymentService(IPaymentRepository paymentRepository, IAthleteRepository athleteRepository)
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IAthleteRepository _athleteRepository = athleteRepository;

    public async Task<IEnumerable<Payment>?> GetPaymentsByEmailAsync(string email)
    {
        EmailAddress? emailAddress = new(email);
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(emailAddress) ??
            throw new InvalidOperationException($"Athlete with email '{email}' not found.");

        IEnumerable<Payment>? payments = await _paymentRepository.GetPaymentByEmailAsync(athlete.EmailAddress);
        return payments;
    }
}