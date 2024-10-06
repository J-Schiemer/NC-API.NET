using System.Net.Http.Headers;
using System.Text;
using NC_API.NET.Containers;
using NC_API.NET.Driver;

namespace NC_API.NET;

/// <summary>
/// A reference to a nextcloud instance
/// </summary>
public class Nextcloud : IDisposable
{
    private readonly string _username;
    private readonly string _apiPassword;
    private readonly string _url;

    private bool _disposed = false;

    private HttpClient? _client;
    private readonly HttpMessageHandler? _handler;

    // For some reason SonarLint thinks that these should be static? Which obviously doesn't work.
#pragma warning disable S2325
    public string Username { get { return _username; } }
    public string ApiPassword { get { return _apiPassword; } }
    public string Url { get { return _url; } }
# pragma warning restore S2325


    /// <summary>
    /// 
    /// </summary>
    /// <param name="username">the username of the user to login as</param>
    /// <param name="password">the users (APP) password - HINT: PREFER APP PASSWORDS FOR SECURITY!</param>
    /// <param name="url">The base url, e.g. https://nc.example.org - NO TRAILING SLASH!</param>
    /// <param name="handler">A message handler for unit testing</param>
    public Nextcloud(string username, string password, string url, HttpMessageHandler? handler = null)
    {
        _username = username;
        _apiPassword = password;
        _url = url;
        _handler = handler;

        RecreateClient();
    }

    public void Dispose()
    {
        Dispose(true);
        // Take this off of finalization queue
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose of the unmanaged resources
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (_client is not null)
            {
                _client.Dispose();
                _client = null;
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Recreates the client in case of a failure
    /// </summary>
    private void RecreateClient()
    {

        try
        {
            if (_client is not null)
            {
                _client.Dispose();
            }
        }
        finally
        {
            if (_handler is not null)
            {
                _client = new HttpClient(_handler);
            }
            else
            {
                _client = new HttpClient();
            }

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_apiPassword}"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }
    }

    /// <summary>
    /// Move a file or folder from source to destination
    /// </summary>
    /// <param name="source">the source path</param>
    /// <param name="destination">the destination path</param>
    /// <param name="overwrite">overwrite destination if it exists</param>
    /// <returns></returns>
    public async Task Move(string source, string destination, bool overwrite = false)
    {
        if (_client is null)
        {
            RecreateClient();
        }

        await WebDavDriver.Move(_client!, _username, _url, source, destination, overwrite);
    }

    /// <summary>
    /// Copy a file or folder from source to destination
    /// </summary>
    /// <param name="source">the source path</param>
    /// <param name="destination">the destination path</param>
    /// <param name="overwrite">overwrite destination if it exists</param>
    /// <returns></returns>
    public async Task Copy(string source, string destination, bool overwrite = false)
    {
        if (_client is null)
        {
            RecreateClient();
        }

        await WebDavDriver.Copy(_client!, _username, _url, source, destination, overwrite);
    }

    /// <summary>
    /// Download a file to a local path
    /// </summary>
    /// <param name="source">the file to download</param>
    /// <param name="destinationPath">the local destination</param>
    /// <returns></returns>
    public async Task Download(string source, string destinationPath)
    {
        if (_client is null)
        {
            RecreateClient();
        }

        await WebDavDriver.Get(_client!, _username, _url, source, destinationPath);
    }

    /// <summary>
    /// List a directorie's contents
    /// </summary>
    /// <param name="source">the directory to list</param>
    /// <returns>a list of folder elements</returns>
    public async Task<List<FolderElement>> ListDirectory(string source)
    {
        if (_client is null)
        {
            RecreateClient();
        }

        return await WebDavDriver.GetContent(_client!, _username, _url, source);
    }

    /// <summary>
    /// Delete the specific resource
    /// </summary>
    /// <param name="target">the resource to delete</param>
    /// <returns></returns>
    public async Task Delete(string target)
    {
        if (_client is null)
        {
            RecreateClient();
        }

        await WebDavDriver.Delete(_client!, _username, _url, target);
    }

    /// <summary>
    /// Uploads a file from the local disk
    /// </summary>
    /// <param name="sourcePath">the path to the file on the local disk</param>
    /// <param name="target">the target to upload to</param>
    /// <returns></returns>
    public async Task Upload(string sourcePath, string target)
    {
        if (_client is null)
        {
            RecreateClient();
        }

        await WebDavDriver.Upload(_client!, _username, _url, sourcePath, target);
    }

    /// <summary>
    /// Create a folder
    /// </summary>
    /// <param name="target">the target to create</param>
    /// <returns></returns>
    public async Task Create(string target)
    {
        if (_client is null)
        {
            RecreateClient();
        }

        await WebDavDriver.CreateFolder(_client!, _username, _url, target);
    }
}
