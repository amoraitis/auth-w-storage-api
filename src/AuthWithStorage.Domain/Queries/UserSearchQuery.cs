namespace AuthWithStorage.Domain.Queries
{
    public sealed class UserSearchQuery : SearchQuery
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Permission { get; set; }
    }
}
