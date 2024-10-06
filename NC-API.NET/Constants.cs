using System;

namespace NC_API.NET;

/// <summary>
/// Contains base path constants, all without TRAILING SLASH
/// </summary>
public static class Constants
{
    public const string BASE_PATH = "/remote.php/dav";
    public const string FILE_BASE = $"{BASE_PATH}/files";

    public const string DAV_NS = "DAV:";
    public const string OC_NS = "http://owncloud.org/ns";
    public const string NC_NS = "http://nextcloud.org/ns";
    public const string SDAV_NS = "http://sabredav.org/ns";


    public const string PROPFIND_XML = """
                <?xml version="1.0"?>
                <d:propfind xmlns:d="DAV:" xmlns:oc="http://owncloud.org/ns" xmlns:nc="http://nextcloud.org/ns">
                    <d:prop>
                            <d:getlastmodified />
                            <d:getetag />
                            <d:getcontenttype />
                            <d:resourcetype />
                            <oc:size />
                            <nc:contained-folder-count />
                            <nc:contained-file-count />
                    </d:prop>
                </d:propfind>
                """;

    public const string RESPONSE_NODE = "response";
    public const string SIZE_NODE = "size";
    public const string FOLDER_COUNT_NODE = "contained-folder-count";
    public const string FILE_COUNT_NODE = "contained-file-count";
    public const string LAST_MODIFIED_NODE = "getlastmodified";
    public const string NAME_NODE = "href";
    public const string CONTENT_TYPE_NODE = "getcontenttype";
    public const string RESOURCE_TYPE_NODE = "resourcetype";
    public const string COLLECTION_NODE = "collection";
    public const string ETAG_NODE = "getetag";
    public const string ERROR_MESSAGE_NODE = "message";
    public const string ERROR_NODE = "exception";
    public const string ERROR_REQUEST_ID_NODE = "request-id";
    public const string ERROR_REMOTE_ADDRESS_NODE = "remote-address";
}
