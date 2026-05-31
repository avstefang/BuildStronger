using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exception;

public class LessonBookedException : System.Exception
{
    public Guid? LessonId { get; } = null;
    public LessonBookedException() { }
    public LessonBookedException(string message) : base(message) { }
    public LessonBookedException(string message, Guid lessonId) : base(message)
    {
        LessonId = lessonId;
    }
}