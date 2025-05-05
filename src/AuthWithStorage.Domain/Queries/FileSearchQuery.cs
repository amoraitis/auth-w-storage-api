namespace AuthWithStorage.Domain.Queries
{
    public class FileSearchQuery : SearchQuery
    {
        public string Name { get; set; }
        public FileType Type { get; set; }
        public int UploadedByUserId { get; set; }
    }
}
