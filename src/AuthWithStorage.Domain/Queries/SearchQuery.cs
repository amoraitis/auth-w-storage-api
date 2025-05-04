namespace AuthWithStorage.Domain.Queries
{
    public class SearchQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "Id";
        public string SortOrder { get; set; } = "ASC"; // or "DESC"
    }
}