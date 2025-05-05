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

    public class FileService : IFileService
    {
        private readonly IRepository<FileModel, int, FileSearchQuery> _fileRepository;
        private readonly IFileStorageService _blobStorageService;
        private readonly CachingService _cachingService;
        private readonly QueryHasher<FileSearchQuery> _queryHasher;

        public FileService(
            IRepository<FileModel, int, FileSearchQuery> fileRepository,
            IFileStorageService blobStorageService,
            CachingService cachingService)
        {
            _fileRepository = fileRepository;
            _blobStorageService = blobStorageService;
            _queryHasher = new QueryHasher<FileSearchQuery>();
            _cachingService = cachingService;
        }

        public async Task<List<FileDto>> GetAllFilesAsync(FileSearchQuery searchQuery)
        {
            var cacheKey = $"_files_{_queryHasher.GetHashKey(searchQuery)}";
            return await _cachingService.GetOrSetAsync(cacheKey, async () =>
            {
                var files = await _fileRepository.GetAllAsync(searchQuery);
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
            var file = await _fileRepository.GetByIdAsync(id);
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
            var filePath = await _blobStorageService.UploadFileAsync(content, fileDto.Name, fileDto.ContentType);

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

            var id = await _fileRepository.AddAsync(fileModel);
            return id;
        }

        public async Task UpdateFileAsync(FileDto fileDto, Stream conent)
        {
            var existingFile = await _fileRepository.GetByIdAsync(fileDto.Id);
            if (existingFile == null) throw new FileNotFoundException("File not found.");

            if (conent != null)
            {
                await _blobStorageService.DeleteFileAsync(existingFile.Path);
                existingFile.Path = await _blobStorageService.UploadFileAsync(conent, fileDto.Name, fileDto.ContentType);
            }

            existingFile.Name = fileDto.Name;
            existingFile.ContentType = fileDto.ContentType;
            existingFile.UpdatedAt = DateTime.UtcNow;

            await _fileRepository.UpdateAsync(existingFile);
        }

        public async Task DeleteFileAsync(int id)
        {
            var file = await _fileRepository.GetByIdAsync(id);
            if (file == null) throw new FileNotFoundException("File not found.");

            await _blobStorageService.DeleteFileAsync(file.Path);
            await _fileRepository.DeleteAsync(id);
        }
    }
}
