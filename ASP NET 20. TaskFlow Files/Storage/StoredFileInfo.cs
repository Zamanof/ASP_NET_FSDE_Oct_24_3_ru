namespace ASP_NET_20._TaskFlow_Files.Storage;

public class StoredFileInfo
{
    public string StorageKey { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public long Size { get; set; }
}
