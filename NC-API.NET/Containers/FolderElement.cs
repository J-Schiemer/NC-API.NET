using System;

namespace NC_API.NET.Containers;

/// <summary>
/// Content of a Folder
/// </summary>
public record FolderElement
{
    /// <summary>
    /// The name of the resource
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The parent path of the folder the resource lies in
    /// </summary>
    public required string? Parent { get; init; }

    /// <summary>
    /// The etag of the resource
    /// </summary>
    public string? Etag { get; init; }

    /// <summary>
    /// The path to the resource
    /// </summary>
    public string? Path
    {
        get
        {
            if (Parent is not null)
            {
                return $"{Parent}/{Name}";
            }
            else
            {
                return null;
            }
        }
    }
    public bool IsFolder { get; init; }
    public string? MimeType { get; init; }
    public long Size { get; init; }

    /// <summary>
    /// The last modified date
    /// </summary>
    public DateTime LastModified { get; init; }

    /// <summary>
    /// Number of contained folders - only useful if element is a folder
    /// </summary>
    public long ContainedFolderCount { get; init; }
    /// <summary>
    /// Number of contained files - only useful if element is a folder
    /// </summary>
    public long ContainedFileCount { get; init; }
}
