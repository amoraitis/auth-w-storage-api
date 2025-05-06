using AuthWithStorage.Application.DTOs;
using AuthWithStorage.Domain.Entities;
using AuthWithStorage.Domain.Queries;
using AuthWithStorage.Infrastructure.Cache;
using AuthWithStorage.Infrastructure.Repositories;
using AuthWithStorage.Infrastructure.Storage;

namespace AuthWithStorage.Application.Services
{
    public interface IFileService
    {
        Task<List<FileDto>> GetAllFilesAsync(FileSearchQuery searchQuery);
        Task<FileDto> GetFileByIdAsync(int id);
        Task<int> AddFileAsync(FileDto fileDto, Stream content);
        Task UpdateFileAsync(FileDto fileDto, Stream content);
        Task DeleteFileAsync(int id);
    }

    public class FileService(IRepository<FileModel, int, FileSearchQuery> fileRepository,
            IFileStorageService blobStorageService,
            CachingService cachingService)
        : IFileService
    {
        private readonly QueryHasher<FileSearchQuery> _queryHasher = new();

        public async Task<List<FileDto>> GetAllFilesAsync(FileSearchQuery searchQuery)
        {
            var cacheKey = $"_files_{_queryHasher.GetHashKey(searchQuery)}";
            return await cachingService.GetOrSetAsync(cacheKey, async () =>
            {
                var files = await fileRepository.GetAllAsync(searchQuery);
                return files.Select(f => new FileDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Path = f.Path,
                    ContentType = f.ContentType,
                    UploadedAt = f.UploadedAt,
                    Size = f.Size
                }).ToList();
            });
        }

        public async Task<FileDto> GetFileByIdAsync(int id)
        {
            var file = await fileRepository.GetByIdAsync(id);
            if (file == null) return null;

            return new FileDto
            {
                Id = file.Id,
                Name = file.Name,
                Path = file.Path,
                ContentType = file.ContentType,
                UploadedAt = file.UploadedAt,
                UploadedByUserId = file.UploadedByUserId,
                Type = file.Type,
                Size = file.Size
            };
        }

        public async Task<int> AddFileAsync(FileDto fileDto, Stream content)
        {
            var filePath = await blobStorageService.UploadFileAsync(content, fileDto.Name, fileDto.ContentType);

            var fileModel = new FileModel
            {
                Name = fileDto.Name,
                Path = filePath,
                ContentType = fileDto.ContentType,
                Size = content.Length,
                UploadedAt = DateTime.UtcNow,
                UploadedByUserId = fileDto.UploadedByUserId,
                Type = fileDto.Type
            };

            var id = await fileRepository.AddAsync(fileModel);
            return id;
        }

        public async Task UpdateFileAsync(FileDto fileDto, Stream conent)
        {
            var existingFile = await fileRepository.GetByIdAsync(fileDto.Id);
            if (existingFile == null) throw new FileNotFoundException("File not found.");

            if (conent != null)
            {
                await blobStorageService.DeleteFileAsync(existingFile.Path);
                existingFile.Path = await blobStorageService.UploadFileAsync(conent, fileDto.Name, fileDto.ContentType);
            }

            existingFile.Name = fileDto.Name;
            existingFile.ContentType = fileDto.ContentType;
            existingFile.UpdatedAt = DateTime.UtcNow;

            await fileRepository.UpdateAsync(existingFile);
        }

        public async Task DeleteFileAsync(int id)
        {
            var file = await fileRepository.GetByIdAsync(id);
            if (file == null) throw new FileNotFoundException("File not found.");

            await blobStorageService.DeleteFileAsync(file.Path);
            await fileRepository.DeleteAsync(id);
        }
    }
}
