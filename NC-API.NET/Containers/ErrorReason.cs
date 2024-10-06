namespace NC_API.NET.Containers;

/// <summary>
/// The error reason of the nextcloud exception
/// </summary>
public enum ErrorReason
{
    /// <summary>
    /// Unspecified error code
    /// </summary>
    UNSPECIFIED,
    /// <summary>
    /// Element not found
    /// </summary>
    NOT_FOUND,
    /// <summary>
    /// Element(s) conflicting
    /// </summary>
    CONFLICT,
    /// <summary>
    /// Bad request to server
    /// </summary>
    BAD_REQUEST,
    /// <summary>
    /// Forbidden for user
    /// </summary>
    FORBIDDEN,
    /// <summary>
    /// Other client errors
    /// </summary>
    CLIENT_ERROR,
    /// <summary>
    /// Serverside errors
    /// </summary>
    SERVER_ERROR
}
