using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exception;

public class EntityAlreadyExistsException : System.Exception
{
    public string EntityName { get; set; }
    public EntityAlreadyExistsException(string message) : base(message)
    {
    }

    public EntityAlreadyExistsException(string entityName, string message) : base(message)
    {
        EntityName = entityName;
    }
}