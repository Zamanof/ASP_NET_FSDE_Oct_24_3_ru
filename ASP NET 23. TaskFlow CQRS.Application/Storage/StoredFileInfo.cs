namespace ASP_NET_23._TaskFlow_CQRS.Application.Storage;

public class StoredFileInfo
{
    public string StorageKey { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public long Size { get; set; }
}
