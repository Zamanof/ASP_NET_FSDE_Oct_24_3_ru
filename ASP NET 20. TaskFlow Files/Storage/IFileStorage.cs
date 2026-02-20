namespace ASP_NET_20._TaskFlow_Files.Storage;
public interface IFileStorage
{
    Task<StoredFileInfo> UploadAsync(
            Stream stream,
            string orginalFileName,
            string contentType,
            string folderKey,
            CancellationToken cancellationToken = default
            );

    Task<Stream> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default
        );
}
