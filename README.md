# NC-API.NET

NC-API.NET is a straightforward API designed for seamless interaction with Nextcloud instances through WebDAV. It enables a variety of operations, including copying, moving, uploading, downloading files, deleting files and directories, listing directory contents, and creating new directories.
Features

    File Operations: Copy, move, upload, and download files.
    Directory Management: Create new directories and delete existing ones.
    Content Listing: Easily list the contents of directories.

## Examples

You can find practical examples in the NC-API.NET-Example project.

## Usage

For detailed usage instructions, please refer to the documentation in the example project.

```csharp
using System.Net.Http.Headers;
using NC_API.NET;

// Create a reference to your Nextcloud instance
var nextcloud = new Nextcloud("username", "app_password", "https://nc.example.org");

// Upload a file
await nextcloud.Upload("local_file_path.txt", "example_dir/remote_file_path.txt");

// Download a file
await nextcloud.Download("remote_file_path.txt", "local_file_path.txt");

// Move a file
await nextcloud.Move("source_path.txt", "example_dir/destination_path.txt", overwrite: true);

// Copy a file
await nextcloud.Copy("source_path.txt", "destination_path_copy.txt", overwrite: true);

// List contents of a directory
var contents = await nextcloud.ListDirectory("directory_path");
foreach (var element in contents)
{
  // Do something here with the items.
}

// Create a new directory
await nextcloud.Create("new_directory");

// Delete a file or directory
await nextcloud.Delete("example_dir_1/example_dir_2/file_to_delete.txt");

```
