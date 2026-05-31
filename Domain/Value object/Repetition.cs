using Domain.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Value_object;

public sealed class Repetition
{
    public int? RepetitionCount { get; private set; }
    public DateTime? RepetitionEndDate { get; private set; }
    public Repetition(int? repetitionCount, DateTime? repetitionEndDate)
    {
        if (repetitionCount.HasValue && repetitionEndDate.HasValue)
        {
            throw new DomainException("Repetition count and end date cannot be set simultaneously.");
        }

        if (repetitionCount.HasValue && repetitionCount <= 0)
        {
            throw new DomainException("Repetition count must be greater than zero.");
        }
        if (repetitionEndDate.HasValue && repetitionEndDate <= DateTime.Now)
        {
            throw new DomainException("Repetition end date must be in the future.");
        }

        RepetitionCount = repetitionCount;
        RepetitionEndDate = repetitionEndDate;
    }
}