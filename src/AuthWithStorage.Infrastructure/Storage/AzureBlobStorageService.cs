using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace AuthWithStorage.Infrastructure.Storage
{
    /// <inheritdoc cref="IFileStorageService"/>/>
    public class AzureBlobStorageService(BlobServiceClient blobServiceClient,
            IOptions<BlobStorageSettings> blobStorageOptions)
        : IFileStorageService
    {
        private readonly BlobStorageSettings _blobStorageSettings = blobStorageOptions.Value;

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ContainerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString());
            var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };

            await blobClient.UploadAsync(fileStream, blobHttpHeaders);
            return blobClient.Uri.ToString();
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                return response.Value?.Content;
            }

            throw new FileNotFoundException($"File '{fileName}' not found in blob storage.");
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<bool> FileExistsAsync(string fileName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var response = await blobClient.ExistsAsync();
            return response.Value;
        }
    }
}