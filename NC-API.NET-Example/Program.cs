using NC_API.NET;

Console.Write("Enter URL (e.g.: https://nc.example.com):");
var url = Console.ReadLine();

Console.Write("Enter Username:");
var user = Console.ReadLine();

Console.Write("Enter Password:");
var password = Console.ReadLine();

if ((user is null) || (password is null) || (url is null))
{
    Console.WriteLine("Username, Password or URL not entered.");
    Console.ReadKey();
    return;
}

using var nextcloud = new Nextcloud(user, password, url);

while (true)
{
    Console.WriteLine("Select Option:\n\t1: List Folder\n\t2: Get File\n\t3: Delete File\n\t4: Move File or Folder\n\t5: Copy File or Folder\n\t6: Upload File\n\t7: Create Folder\n\t8: Exit\n\nSelect: ");
    var selection = Console.ReadLine();

    switch (selection)
    {
        case "1": await ListFolders(nextcloud); break;
        case "2": await GetFile(nextcloud); break;
        case "3": await DeleteFile(nextcloud); break;
        case "4": await Move(nextcloud); break;
        case "5": await Copy(nextcloud); break;
        case "6": await Upload(nextcloud); break;
        case "7": await CreateFolder(nextcloud); break;
        case "8": return;
        default: Console.WriteLine("Unknown Option."); break;
    }
}

async Task ListFolders(Nextcloud nextcloud)
{
    Console.Write("\n------ List Folder ------\nEnter Remote Path: ");
    var path = Console.ReadLine() ?? "/";

    try
    {
        var result = await nextcloud.ListDirectory(path);
        Console.WriteLine("Contents:");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Name}:\n\tSize: {item.Size}\n\tEtag: {item.Etag}\n\tPath: {item.Path}\n\tLast Modified: {item.LastModified}");

            if (item.IsFolder)
            {
                Console.WriteLine($"\tType: Folder\n\tContained Folders: {item.ContainedFolderCount}\n\tContained Files: {item.ContainedFileCount}");
            }
            else
            {
                Console.WriteLine($"\tType: File\n\tMime Type: {item.MimeType}");
            }
        }
    }
    catch (NextcloudException e)
    {
        Console.WriteLine($"Error: {e.Message}\n {e.InnerException?.Message ?? ""}\n{e.StackTrace}");
    }
}

async Task GetFile(Nextcloud nextcloud)
{
    Console.Write("\n------ Get File ------\nEnter Remote Path: ");
    var path = Console.ReadLine();
    Console.Write("\nEnter Local Path: ");
    var local = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(local))
    {
        Console.WriteLine("Paths not provided.");
        return;
    }

    try
    {
        await nextcloud.Download(path, local);
    }
    catch (NextcloudException e)
    {
        Console.WriteLine($"Error: {e.Message}\n {e.InnerException?.Message ?? ""}\n{e.StackTrace}");
    }
}

async Task DeleteFile(Nextcloud nextcloud)
{
    Console.Write("\n------ Delete Folder/File ------\nEnter Path: ");
    var path = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(path))
    {
        Console.WriteLine("Paths not provided.");
        return;
    }

    try
    {
        await nextcloud.Delete(path);
    }
    catch (NextcloudException e)
    {
        Console.WriteLine($"Error: {e.Message}\n {e.InnerException?.Message ?? ""}\n{e.StackTrace}");
    }
}

async Task CreateFolder(Nextcloud nextcloud)
{
    Console.Write("\n------ Create Folder ------\nEnter Path: ");
    var path = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(path))
    {
        Console.WriteLine("Path not provided.");
        return;
    }

    try
    {
        await nextcloud.Create(path);
    }
    catch (NextcloudException e)
    {
        Console.WriteLine($"Error: {e.Message}\n {e.InnerException?.Message ?? ""}\n{e.StackTrace}");
    }
}

async Task Move(Nextcloud nextcloud)
{
    Console.Write("\n------ Move Folder/File ------\nEnter Source Path: ");
    var source = Console.ReadLine();
    Console.Write("\nEnter Target Path: ");
    var destination = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination))
    {
        Console.WriteLine("Paths not provided.");
        return;
    }

    try
    {
        await nextcloud.Move(source, destination);
    }
    catch (NextcloudException e)
    {
        Console.WriteLine($"Error: {e.Message}\n {e.InnerException?.Message ?? ""}\n{e.StackTrace}");
    }
}

async Task Copy(Nextcloud nextcloud)
{
    Console.Write("\n------ Copy Folder/File ------\nEnter Source Path: ");
    var source = Console.ReadLine();
    Console.Write("\nEnter Target Path: ");
    var destination = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination))
    {
        Console.WriteLine("Paths not provided.");
        return;
    }

    try
    {
        await nextcloud.Copy(source, destination);
    }
    catch (NextcloudException e)
    {
        Console.WriteLine($"Error: {e.Message}\n {e.InnerException?.Message ?? ""}\n{e.StackTrace}");
    }
}

async Task Upload(Nextcloud nextcloud)
{
    Console.Write("\n------ Upload File ------\nEnter Local Path: ");
    var local = Console.ReadLine();
    Console.Write("\nEnter Remote Path: ");
    var target = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(local) || string.IsNullOrWhiteSpace(target))
    {
        Console.WriteLine("Paths not provided.");
        return;
    }

    try
    {
        await nextcloud.Upload(local, target);
    }
    catch (NextcloudException e)
    {
        Console.WriteLine($"Error: {e.Message}\n {e.InnerException?.Message ?? ""}\n{e.StackTrace}");
    }
}