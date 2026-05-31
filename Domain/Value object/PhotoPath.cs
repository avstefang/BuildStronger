using System;
using System.Collections.Generic;
using System.Text;
using Domain.Exception;

namespace Domain.Value_object;

public sealed record class PhotoPath
{
    public string? Path { get; }
    public PhotoPath(string? path)
    {
        if (path == null) return;

        if (!IsPathValid(path))
        {
            throw new DomainException("The provided photo path is invalid.", path);
        }

        Path = path;
    }

    internal bool IsPathValid(string path) => (string.IsNullOrWhiteSpace(path) ||
            path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0) == false;
}