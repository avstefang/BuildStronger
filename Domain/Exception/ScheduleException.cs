using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exception;

public class ScheduleException : System.Exception
{
    public ScheduleException(string message) : base(message) { }
}