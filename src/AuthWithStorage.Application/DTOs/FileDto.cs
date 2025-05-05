using AuthWithStorage.Domain;
using Microsoft.AspNetCore.Http;

namespace AuthWithStorage.Application.DTOs
{
    public class FileDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string ContentType { get; set; }
        public FileType Type { get; set; }
        public long Size { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class FileRequest
    {
        public string Name { get; set; }

        public FileType Type { get; set; }

        public int UploadedByUserId { get; set; }
        public IFormFile FormFile { get; set; }
        public string ContentType { get; set; }
    }
}
