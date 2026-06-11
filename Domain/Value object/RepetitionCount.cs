using Domain.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Value_object;

public class RepetitionCount
{
    public int Value { get; private set; }

    public RepetitionCount(int value)
    {
        if (value < 1)
        {
            throw new DomainException("Repetition always has to be at least set to 1.");
        }

        Value = value;
    }
}