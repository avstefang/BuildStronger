using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Value_object;

public class ProcessorId
{
    public string Id { get; private set; }

    public ProcessorId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Processor ID cannot be null or empty.", nameof(id));
        }

        if (id.Length < 15)
            throw new ArgumentException("Processor ID must be at least 15 characters long.", nameof(id));

        Id = id;
    }
}