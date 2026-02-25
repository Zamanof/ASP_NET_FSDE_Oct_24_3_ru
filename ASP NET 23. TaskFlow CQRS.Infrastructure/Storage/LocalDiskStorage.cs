using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ASP_NET_23._TaskFlow_CQRS.Application.Storage;

namespace ASP_NET_23._TaskFlow_CQRS.Infrastructure.Storage;

public class LocalDiskStorage : IFileStorage
{
    private readonly string _basePath;
    private readonly ILogger<LocalDiskStorage> _logger;

    public LocalDiskStorage(IHostEnvironment env, ILogger<LocalDiskStorage> logger)
    {
        _basePath = Path.Combine(env.ContentRootPath ?? AppDomain.CurrentDomain.BaseDirectory, "Storage");
        _logger = logger;
    }

    public async Task<StoredFileInfo> UploadAsync(Stream stream, string originalFileName, string contentType, string folderKey, CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(originalFileName);
        if (string.IsNullOrEmpty(ext)) ext = ".bin";

        var storedFileName = $"{Guid.NewGuid():N}{ext}";
        var relativePath = Path.Combine(folderKey, storedFileName).Replace('\\', '/');
        var fullPath = Path.Combine(_basePath, folderKey, storedFileName);
        var dir = Path.GetDirectoryName(fullPath);
        Directory.CreateDirectory(dir!);

        await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            await stream.CopyToAsync(fs, cancellationToken);

        var size = new FileInfo(fullPath).Length;
        _logger.LogInformation("Uploaded file to {Path}, size {Size}", fullPath, size);

        return new StoredFileInfo
        {
            StorageKey = relativePath,
            StoredFileName = storedFileName,
            Size = size
        };
    }

    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, storageKey.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("File not found in storage", storageKey);
        return Task.FromResult<Stream>(new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read));
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
            _logger.LogWarning("Could not delete file {Path}", fullPath);
        return Task.CompletedTask;
    }
}
