using System;
using System.Runtime.Serialization;

namespace NC_API.NET;

/// <summary>
/// An Exception within Nextcloud
/// </summary>
public class NextcloudException : Exception
{
    public NextcloudException()
    {
    }

    public NextcloudException(string? message) : base(message)
    {
    }

    public NextcloudException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
