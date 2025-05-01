using System;
using AuthWithStorage.Domain.Enums;

namespace AuthWithStorage.Domain.Entities
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string ContentType { get; set; }
        public FileType Type { get; set; }
        public long Size { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}