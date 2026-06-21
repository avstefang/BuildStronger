using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public sealed record EmailContentDto(string Subject, string Body)
{
    public string Subject { get; private set; } = Subject;
    public string Body { get; private set; } = Body;
}