using System;
using System.Runtime.Serialization;
using NC_API.NET.Containers;

namespace NC_API.NET;

/// <summary>
/// An Exception within Nextcloud
/// </summary>
public class NextcloudException : Exception
{
    /// <summary>
    /// Contains the reason for the error, can be used to determine whether to retry
    /// </summary>
    public ErrorReason ErrorReason { get; set; } = ErrorReason.UNSPECIFIED;

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
