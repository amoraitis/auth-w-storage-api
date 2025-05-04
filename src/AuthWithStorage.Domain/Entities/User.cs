namespace AuthWithStorage.Domain.Entities
{
    public class User : IEntity<int>
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public PermissionType[] Permissions { get; set; }
        public string Role { get; set; }
    }
}
