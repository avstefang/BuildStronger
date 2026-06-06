using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Dto;

public class UpdateLessonDto
{
    public Guid Id { get; private set; }
    public Schedule? Schedule { get; private set; }
    public int? MaxCapacity { get; private set; }
    public int? CustomDuration { get; private set; }

    public UpdateLessonDto(Guid id, Schedule? schedule = null, int? maxCapacity = null, int? customDuration = null)
    {
        if (maxCapacity != null && maxCapacity < 0)
            throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Max capacity cannot be negative.");

        if (customDuration != null && customDuration < 0)
            throw new ArgumentOutOfRangeException(nameof(customDuration), "Custom duration cannot be negative.");

        if (schedule == null && maxCapacity == null && customDuration == null)
            throw new ArgumentException("At least one property must be provided for update.");

        Id = id;
        Schedule = schedule;
        MaxCapacity = maxCapacity;
        CustomDuration = customDuration;
    }
}