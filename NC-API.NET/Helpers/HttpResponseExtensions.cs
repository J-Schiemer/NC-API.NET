using System;
using NC_API.NET.Containers;

namespace NC_API.NET.Helpers;

/// <summary>
/// Extension methods for HttpResponse
/// </summary>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Ensures the success status of the message or throws a nextcloud exception
    /// </summary>
    /// <exception cref="NextcloudException"></exception>
    public static void EnsureSucessOrThrowNcException(this HttpResponseMessage response)
    {
        try
        {
            // Get Detailed Exception
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            var code = (int)response.StatusCode;
            ErrorReason reason = ErrorReason.UNSPECIFIED;

            // Generic Code Ranges
            if (code >= 500)
            {
                reason = ErrorReason.SERVER_ERROR;
            }
            else if (code >= 400)
            {
                reason = ErrorReason.CLIENT_ERROR;
            }

            // Special Codes
            if (code == 409)
            {
                reason = ErrorReason.CONFLICT;
            }
            else if (code == 404)
            {
                reason = ErrorReason.NOT_FOUND;
            }
            else if (code == 400)
            {
                reason = ErrorReason.BAD_REQUEST;
            }
            else if (code == 403)
            {
                reason = ErrorReason.FORBIDDEN;
            }
            throw new NextcloudException("Operation Failed", e) { ErrorReason = reason };
        }
    }

}
