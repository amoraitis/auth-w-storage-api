namespace AuthWithStorage.Infrastructure.Storage
{
    /// <summary>
    /// Defines a service for file storage operations such as uploading, downloading, deleting, and checking file existence.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads a file to the storage.
        /// </summary>
        /// <param name="fileStream">The stream containing the file data to upload.</param>
        /// <param name="fileName">The name of the file to be stored.</param>
        /// <param name="contentType">The MIME type of the file.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the URL or identifier of the uploaded file.</returns>
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);

        /// <summary>
        /// Downloads a file from the storage.
        /// </summary>
        /// <param name="fileName">The name of the file to download.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the stream of the downloaded file.</returns>
        Task<Stream> DownloadFileAsync(string fileName);

        /// <summary>
        /// Deletes a file from the storage.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteFileAsync(string fileName);

        /// <summary>
        /// Checks if a file exists in the storage.
        /// </summary>
        /// <param name="fileName">The name of the file to check.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the file exists.</returns>
        Task<bool> FileExistsAsync(string fileName);
    }
}