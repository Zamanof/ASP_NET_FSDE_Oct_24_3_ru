namespace ASP_NET_23._TaskFlow_CQRS.Application.Storage;

public interface IFileStorage
{
    Task<StoredFileInfo> UploadAsync(
        Stream stream,
        string originalFileName,
        string contentType,
        string folderKey,
        CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}
