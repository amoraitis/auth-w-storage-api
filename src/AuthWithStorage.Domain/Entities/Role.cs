namespace AuthWithStorage.Domain.Entities
{
    public class Role : IEntity<UserRole>
    {
        public UserRole Id { get; set; }
        public string Name { get; set; }
    }
}
