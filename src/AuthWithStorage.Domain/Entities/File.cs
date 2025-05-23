﻿namespace AuthWithStorage.Domain.Entities
{
    public class FileModel : IEntity<int>
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

        public string UploadedByUsername { get; set; }
    }
}