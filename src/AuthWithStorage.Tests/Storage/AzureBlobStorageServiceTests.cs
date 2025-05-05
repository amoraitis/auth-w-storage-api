using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AuthWithStorage.Infrastructure.Storage;
using Microsoft.Extensions.Options;
using Moq;
using System.Text;
using Azure;

namespace AuthWithStorage.Tests.Storage
{
    public class AzureBlobStorageServiceTests
    {
        private readonly Mock<BlobClient> _blobClientMock;
        private readonly AzureBlobStorageService _service;

        public AzureBlobStorageServiceTests()
        {
            var blobServiceClientMock = new Mock<BlobServiceClient>();
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            _blobClientMock = new Mock<BlobClient>();

            blobServiceClientMock
                .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(blobContainerClientMock.Object);

            blobContainerClientMock
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(_blobClientMock.Object);

            var blobStorageSettings = new BlobStorageSettings { ContainerName = "test-container" };
            var optionsMock = new Mock<IOptions<BlobStorageSettings>>();
            optionsMock.Setup(o => o.Value).Returns(blobStorageSettings);

            _service = new AzureBlobStorageService(blobServiceClientMock.Object, optionsMock.Object);
        }

        [Fact]
        public async Task UploadFileAsync_ShouldReturnBlobUri()
        {
            // Arrange
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
            var contentType = "text/plain";
            var blobUri = new Uri("https://example.com/blob");

            _blobClientMock
                .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobHttpHeaders>(), null, null, null, default, default, default))
                .ReturnsAsync(Response.FromValue((BlobContentInfo)null, null)); // Fixed return value to match UploadAsync signature

            _blobClientMock
                .Setup(x => x.Uri)
                .Returns(blobUri);

            // Act
            var result = await _service.UploadFileAsync(fileStream, "test.txt", contentType);

            // Assert
            Assert.Equal(blobUri.ToString(), result);
        }

        [Fact]
        public async Task DownloadFileAsync_ShouldReturnFileStream_WhenFileExists()
        {
            // Arrange
            var fileContent = "test content";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            var blobDownloadInfo = BlobsModelFactory.BlobDownloadInfo(content: fileStream);

            _blobClientMock
                .Setup(x => x.ExistsAsync(default))
                .ReturnsAsync(Response.FromValue(true, null));

            var mockResponse = new Mock<Response<BlobDownloadInfo>>();
            mockResponse.Setup(r => r.Value).Returns(blobDownloadInfo);

            _blobClientMock
                .Setup(x => x.DownloadAsync())
                .ReturnsAsync(mockResponse.Object);

            // Act
            var result = await _service.DownloadFileAsync("test.txt");

            // Assert
            using var reader = new StreamReader(result);
            var content = await reader.ReadToEndAsync();
            Assert.Equal(fileContent, content);
        }

        [Fact]
        public async Task DownloadFileAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            _blobClientMock
                .Setup(x => x.ExistsAsync(default))
                .ReturnsAsync(Response.FromValue(false, null));

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _service.DownloadFileAsync("nonexistent.txt"));
        }

        [Fact]
        public async Task DeleteFileAsync_ShouldCallDeleteIfExistsAsync()
        {
            // Arrange
            _ = _blobClientMock
                .Setup(x => x.DeleteIfExistsAsync(default, default, default))
                .ReturnsAsync(Response.FromValue(true, null));

            // Act
            await _service.DeleteFileAsync("test.txt");

            // Assert
            _blobClientMock.Verify(x => x.DeleteIfExistsAsync(default, default, default), Times.Once);
        }

        [Fact]
        public async Task FileExistsAsync_ShouldReturnTrue_WhenFileExists()
        {
            // Arrange
            _blobClientMock
                .Setup(x => x.ExistsAsync(default))
                .ReturnsAsync(Response.FromValue(true, null));

            // Act
            var result = await _service.FileExistsAsync("test.txt");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task FileExistsAsync_ShouldReturnFalse_WhenFileDoesNotExist()
        {
            // Arrange
            _blobClientMock
                .Setup(x => x.ExistsAsync(default))
                .ReturnsAsync(Response.FromValue(false, null));

            // Act
            var result = await _service.FileExistsAsync("nonexistent.txt");

            // Assert
            Assert.False(result);
        }

        // Added missing edge case test
        [Fact]
        public async Task UploadFileAsync_ShouldThrowException_WhenUploadFails()
        {
            // Arrange
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
            var contentType = "text/plain";

            _blobClientMock
                .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobHttpHeaders>(), null, null, null, default, default, default))
                .ThrowsAsync(new RequestFailedException("Upload failed"));

            // Act & Assert
            await Assert.ThrowsAsync<RequestFailedException>(() => _service.UploadFileAsync(fileStream, "test.txt", contentType));
        }
    }
}
