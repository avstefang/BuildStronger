using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exception;

public class InvalidPhotoPathException : System.Exception
{
    public string Path { get; }
    public InvalidPhotoPathException() { }
    public InvalidPhotoPathException(string message) : base(message) { }
    public InvalidPhotoPathException(string message, string path) : base(message)
    {
        Path = path;
    }
}