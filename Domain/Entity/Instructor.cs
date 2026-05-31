using Domain.Exception;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity;

public class Instructor
{
    public Athlete Athlete { get; }

    public Instructor(Athlete athlete)
    {
        if (!athlete.IsInstructor())
            throw new InstructorNotFoundException("The specified athlete does not have the role of Instructor.", athlete.FullName.ToString());

        Athlete = athlete;
    }
}