
using ASP_NET_21._TaskFlow.BLL.Storage;
using System.Linq.Expressions;

namespace ASP_NET_21._TaskFlow.API.Storage;

public class LocalDiskStorage : IFileStorage
{
    private readonly string _basePath;
    private readonly ILogger<LocalDiskStorage> _logger;

    public LocalDiskStorage(IWebHostEnvironment env, ILogger<LocalDiskStorage> logger)
    {
        _basePath = Path.Combine(env.ContentRootPath, "Storage");
        _logger = logger;
    }

    public async Task<StoredFileInfo> UploadAsync(
        Stream stream, 
        string originalFileName, 
        string contentType, 
        string folderKey, 
        CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(originalFileName);
        if (string.IsNullOrEmpty(ext))
            ext = ".bin";

        var storedFileName = $"{Guid.NewGuid():N}{ext}";

        var relativePath = Path.Combine(folderKey, storedFileName);

        var fullPath = Path.Combine(_basePath, relativePath);

        var dir = Path.GetDirectoryName(fullPath);

        Directory.CreateDirectory(dir!);

        await using (var fs = new FileStream(
                                            fullPath,
                                            FileMode.Create,
                                            FileAccess.Write,
                                            FileShare.None,
                                            4096,
                                            useAsync: true))
        {
            await stream.CopyToAsync(fs, cancellationToken);
        }

        var size = new FileInfo(fullPath).Length;

        _logger.LogInformation("Uploaded file to {Path}, size {Size}", fullPath, size);

        return new StoredFileInfo
        {
            StorageKey = relativePath.Replace('\\', '/'),
            StoredFileName = storedFileName,
            Size = size
        };
    }
    
    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, storageKey.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("File not found in storage", storageKey);

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        return Task.FromResult<Stream>(stream);
    }
    
    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, storageKey.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Deleted file {Path}", fullPath);
        }
        else
        {
            _logger.LogWarning("Could not delete file {Path}", fullPath);
        }
        return Task.CompletedTask;
    }

}
