using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Xml.Linq;
using NC_API.NET.Containers;
using Microsoft.AspNetCore.StaticFiles;
using NC_API.NET.Helpers;

namespace NC_API.NET.Driver;

/// <summary>
/// Contains WebDav Commands
/// </summary>
public static class WebDavDriver
{
    /// <summary>
    /// Get a folder's content
    /// </summary>
    /// <param name="client">The http client to use in this request</param>
    /// <param name="username">the user the request is made for</param>
    /// <param name="url">the url without trailing slash the nextcloud instance is running at</param>
    /// <param name="path">the path to the file in the users directory</param>
    /// <param name="depth">the search depth, default 1</param>
    /// <returns>returns a list of folder elements</returns>
    /// <exception cref="ArgumentOutOfRangeException">thrown if the dpeth parameter is negative</exception>
    /// <exception cref="NextcloudException">thrown if something else goes wrong, wraps inner exceptions</exception>
    public static async Task<List<FolderElement>> GetContent(HttpClient client, string username, string url, string path, int depth = 1)
    {
        if (depth < 0)
        {
            throw new ArgumentOutOfRangeException("depth", "Depth must not be negative.");
        }

        string folderPath = $"{url}{Constants.FILE_BASE}/{username}/{path}";

        try
        {
            // Depth tells nextcloud how many layers deep to check. Depth 0 only checks the folder at path itself
            var request = new HttpRequestMessage(new HttpMethod("PROPFIND"), folderPath);
            request.Headers.Add("Depth", $"{depth}");

            request.Content = new StringContent(Constants.PROPFIND_XML, Encoding.UTF8, "application/xml");
            HttpResponseMessage response = await client.SendAsync(request);

            response.EnsureSucessOrThrowNcException();

            // Parse Response
            string responseContent = await response.Content.ReadAsStringAsync();
            return ParseResponse(responseContent, path);
        }
        catch (Exception e) when (e is not NextcloudException)
        {
            throw new NextcloudException("Fetching folder Content failed", e);
        }
    }

    /// <summary>
    /// Move a file or folder
    /// </summary>
    /// <param name="client">The http client to use in this request</param>
    /// <param name="username">the username of the user to perform this action with</param>
    /// <param name="url">the url of the nextcloud instance without trailing slash</param>
    /// <param name="source">the source path of the file or folder</param>
    /// <param name="target">the target path of the file or folder</param>
    /// <param name="overwrite">overwrite at target iff true</param>
    /// <returns></returns>
    /// <exception cref="NextcloudException">thrown if an exception occurs, wraps inner exception</exception>
    public static async Task Move(HttpClient client, string username, string url, string source, string target, bool overwrite = false)
    {
        string sourcePath = $"{url}{Constants.FILE_BASE}/{username}/{source}";
        string targetPath = $"{url}{Constants.FILE_BASE}/{username}/{target}";

        try
        {
            var request = new HttpRequestMessage(new HttpMethod("MOVE"), sourcePath);
            request.Headers.Add("Destination", targetPath);
            request.Headers.Add("Overwrite", overwrite ? "T" : "F");

            HttpResponseMessage response = await client.SendAsync(request);

            response.EnsureSucessOrThrowNcException();
        }
        catch (Exception e) when (e is not NextcloudException)
        {
            throw new NextcloudException("Moving element failed", e);
        }
    }

    /// <summary>
    /// Copy a file or folder
    /// </summary>
    /// <param name="client">The http client to use in this request</param>
    /// <param name="username">the username of the user to perform this action with</param>
    /// <param name="url">the url of the nextcloud instance without trailing slash</param>
    /// <param name="source">the source path of the file or folder</param>
    /// <param name="target">the target path of the file or folder</param>
    /// <param name="overwrite">overwrite at target iff true</param>
    /// <returns></returns>
    /// <exception cref="NextcloudException">thrown if an exception occurs, wraps inner exception</exception>
    public static async Task Copy(HttpClient client, string username, string url, string source, string target, bool overwrite = false)
    {
        string sourcePath = $"{url}{Constants.FILE_BASE}/{username}/{source}";
        string targetPath = $"{url}{Constants.FILE_BASE}/{username}/{target}";

        try
        {
            var request = new HttpRequestMessage(new HttpMethod("COPY"), sourcePath);
            request.Headers.Add("Destination", targetPath);
            request.Headers.Add("Overwrite", overwrite ? "T" : "F");

            HttpResponseMessage response = await client.SendAsync(request);

            response.EnsureSucessOrThrowNcException();
        }
        catch (Exception e) when (e is not NextcloudException)
        {
            throw new NextcloudException("Copying element failed", e);
        }
    }

    /// <summary>
    /// Deletes the file or folder at source
    /// </summary>
    /// <param name="client">The http client to use in this request</param>
    /// <param name="username">the username of the user to perform this action with</param>
    /// <param name="url">the url of the nextcloud instance without trailing slash</param>
    /// <param name="source">the source path of the file</param>
    /// <returns></returns>
    /// <exception cref="NextcloudException">thrown if the download failed, wraps inner exception</exception>
    public static async Task Delete(HttpClient client, string username, string url, string source)
    {
        string sourcePath = $"{url}{Constants.FILE_BASE}/{username}/{source}";

        try
        {
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), sourcePath);

            HttpResponseMessage response = await client.SendAsync(request);

            response.EnsureSucessOrThrowNcException();
        }
        catch (Exception e) when (e is not NextcloudException)
        {
            throw new NextcloudException("Deleting element failed", e);
        }
    }


    /// <summary>
    /// Create a new folder
    /// </summary>
    /// <param name="client">the http client to use</param>
    /// <param name="username">the username of the logged in user</param>
    /// <param name="url">the url of the server</param>
    /// <param name="target">the target folder</param>
    /// <returns></returns>
    /// <exception cref="NextcloudException"></exception>
    public static async Task CreateFolder(HttpClient client, string username, string url, string target)
    {
        string targetPath = $"{url}{Constants.FILE_BASE}/{username}/{target}";

        try
        {
            var request = new HttpRequestMessage(new HttpMethod("MKCOL"), targetPath);

            HttpResponseMessage response = await client.SendAsync(request);

            response.EnsureSucessOrThrowNcException();
        }
        catch (Exception e) when (e is not NextcloudException)
        {
            throw new NextcloudException("Creating folder failed", e);
        }
    }

    /// <summary>
    /// Downloads a file into the local target path
    /// </summary>
    /// <param name="client">The http client to use in this request</param>
    /// <param name="username">the username of the user to perform this action with</param>
    /// <param name="url">the url of the nextcloud instance without trailing slash</param>
    /// <param name="source">the source path of the file</param>
    /// <param name="localTarget">the local target path</param>
    /// <returns></returns>
    /// <exception cref="NextcloudException">thrown if the download failed, wraps inner exception</exception>
    public static async Task Get(HttpClient client, string username, string url, string source, string localTarget)
    {
        string sourcePath = $"{url}{Constants.FILE_BASE}/{username}/{source}";

        try
        {
            var request = new HttpRequestMessage(new HttpMethod("GET"), sourcePath);

            HttpResponseMessage response = await client.SendAsync(request);

            response.EnsureSucessOrThrowNcException();

            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
            {
                using (FileStream fileStream = new FileStream(localTarget, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await contentStream.CopyToAsync(fileStream);
                }
            }

        }
        catch (Exception e) when (e is not NextcloudException)
        {
            throw new NextcloudException("Fetching element failed", e);
        }
    }

    /// <summary>
    /// Uploads a file into from local source path
    /// </summary>
    /// <param name="client">The http client to use in this request</param>
    /// <param name="username">the username of the user to perform this action with</param>
    /// <param name="url">the url of the nextcloud instance without trailing slash</param>
    /// <param name="source">the target path of the file</param>
    /// <param name="localTarget">the local source path</param>
    /// <returns></returns>
    /// <exception cref="NextcloudException">thrown if the download failed, wraps inner exception</exception>
    public static async Task Upload(HttpClient client, string username, string url, string localSource, string target)
    {
        string sourcePath = $"{url}{Constants.FILE_BASE}/{username}/{target}";

        try
        {
            var content = new StreamContent(File.OpenRead(localSource));
            new FileExtensionContentTypeProvider().TryGetContentType(target, out string? contentType);
            if (contentType is not null)
            {
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }

            var response = await client.PutAsync(sourcePath, content);

            response.EnsureSucessOrThrowNcException();
        }
        catch (Exception e) when (e is not NextcloudException)
        {
            throw new NextcloudException("Fetching element failed", e);
        }
    }

    /// <summary>
    /// Parses a Propfind Response into a list of folder elements
    /// </summary>
    /// <param name="response">The response to parse</param>
    /// <param name="path">the path of the propfind request</param>
    /// <returns>Returns a list of folder elements</returns>
    private static List<FolderElement> ParseResponse(string response, string path)
    {
        XDocument xmlDoc = XDocument.Parse(response);
        XNamespace d = Constants.DAV_NS;
        XNamespace oc = Constants.OC_NS;
        XNamespace nc = Constants.NC_NS;
        XNamespace s = Constants.SDAV_NS;

        // Check if the response is an error response and handle if yes
        var error = xmlDoc.Descendants(s + Constants.ERROR_NODE).FirstOrDefault()?.Value;
        if (error is not null)
        {
            var message = xmlDoc.Descendants(s + Constants.ERROR_MESSAGE_NODE).FirstOrDefault()?.Value.Trim() ?? "An error occurred";
            var remoteAddress = xmlDoc.Descendants(s + Constants.ERROR_REMOTE_ADDRESS_NODE).FirstOrDefault()?.Value;
            var requestId = xmlDoc.Descendants(s + Constants.ERROR_REQUEST_ID_NODE).FirstOrDefault()?.Value;

            throw new NextcloudException($"An error occured: {error}\n{message}\nDetails:\n\tAddress: {remoteAddress}\n\tRequest ID: {requestId}");
        }

        // otherwise, parse all values into FolderEntries
        var items = xmlDoc.Descendants(d + Constants.RESPONSE_NODE).Where(item => { // Skip Current Dir
            var href = item.Descendants(d + Constants.NAME_NODE).FirstOrDefault()?.Value ?? "";
            return !(href.EndsWith(path) || href.EndsWith($"{path}/")); // Current Directory
        }).Select(item =>
        {
            // The last part of the href is the filename or foldername!
            var href = item.Descendants(d + Constants.NAME_NODE).FirstOrDefault()?.Value ?? "";
            var name = href.TrimEnd('/').Split('/').LastOrDefault() ?? "NONAME";

            // Element Size in Bytes
            var size = item.Descendants(oc + Constants.SIZE_NODE).FirstOrDefault()?.Value ?? "0";
            long.TryParse(size, out long sizeLong);

            // Number of folders contained in element (notfound if element does not exist -> cast to 0)
            var folderCountValue = item.Descendants(nc + Constants.FOLDER_COUNT_NODE).FirstOrDefault()?.Value ?? "0";
            long.TryParse(folderCountValue, out long folderCount);

            // Number of files contained in element (notfound if element does not exist -> cast to 0)
            var fileCountValue = item.Descendants(nc + Constants.FILE_COUNT_NODE).FirstOrDefault()?.Value ?? "0";
            long.TryParse(fileCountValue, out long fileCount);

            // Content type will be null if the element is a folder -> replace with emtpy string
            var contentType = item.Descendants(d + Constants.CONTENT_TYPE_NODE).FirstOrDefault()?.Value ?? "";

            // Check if element has the resource type collection -> if yes, it's a folder
            var resourcetype = item.Descendants(d + Constants.RESOURCE_TYPE_NODE).FirstOrDefault()?.Descendants(d + Constants.COLLECTION_NODE).Any() == true ? "Collection" : "File";
            bool isFolder = resourcetype == "Collection";

            // Last modified date
            var lastModifiedValue = item.Descendants(d + Constants.LAST_MODIFIED_NODE).FirstOrDefault()?.Value ?? "";
            DateTime? lastModified = null;

            try
            {
                lastModified = Convert.ToDateTime(lastModifiedValue);
            }
            catch (FormatException)
            {
                lastModified = DateTime.UtcNow;
            }

            return new FolderElement()
            {
                Etag = item.Descendants(d + Constants.ETAG_NODE).FirstOrDefault()?.Value,
                Size = sizeLong,
                Parent = path,
                Name = name,
                ContainedFileCount = fileCount,
                ContainedFolderCount = folderCount,
                LastModified = (DateTime)lastModified,
                MimeType = contentType,
                IsFolder = isFolder
            };
        });

        return items.ToList();
    }
}
