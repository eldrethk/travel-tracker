using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System.Security;

namespace TravelExpenseTracker.Services
{
    public interface IBlobService 
    {
        Task<string> UploadFile(IFormFile file, string containerName, CancellationToken ct = default);
        Task<string> GetFile(string containerName, string blobName, CancellationToken ct = default);
        Task DeleteFile(string blobName, string containerName, CancellationToken ct =default);
    }

    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IContainerNameResolver _containerResolver;
        private static readonly HashSet<string> AllowedExts =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };

        public BlobService(BlobServiceClient blobServiceClient, IContainerNameResolver containerName)
        {
           _blobServiceClient = blobServiceClient;
            _containerResolver = containerName;
        }

        private async Task<BlobContainerClient> GetBlobContainer(string containerKey, CancellationToken ct = default)
        {
            
            var actualContainerName = _containerResolver.Resolve(containerKey);
            var container = _blobServiceClient.GetBlobContainerClient(actualContainerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);
            return container;
        }
        

        public async Task DeleteFile(string blobName, string containerName, CancellationToken ct = default)
        {
            var container = await GetBlobContainer(containerName, ct);
            await container.DeleteBlobIfExistsAsync(blobName, cancellationToken: ct);
        }

        public async Task<string> GetFile(string containerName, string blobName, CancellationToken ct = default)
        {
            var container = await GetBlobContainer(containerName, ct);
            var blob = container.GetBlobClient(blobName);

            if (!blob.CanGenerateSasUri)
                throw new InvalidOperationException("Cannot find file with current credentials");

           return blob.Uri.ToString();
        }

        public async Task<string> UploadFile(IFormFile file, string containerName, CancellationToken ct = default)
        {
            if (file is null || file.Length == 0) throw new ArgumentException("Empty file.");
            if (file.Length > 10 * 1024 * 1024) throw new InvalidOperationException("File too large (10MB max).");

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedExts.Contains(ext)) throw new InvalidOperationException($"Unsupported file type: {ext}");

            var container = await GetBlobContainer(containerName, ct);

            var blobName = $"{DateTime.Now:mm_dd_yy}_{Guid.NewGuid():N}{ext}";
            var blob = container.GetBlobClient(blobName);

            await using (Stream s = file.OpenReadStream())
            {
                await blob.UploadAsync(s, ct);
            }
            return blobName;
        }
    }
}
