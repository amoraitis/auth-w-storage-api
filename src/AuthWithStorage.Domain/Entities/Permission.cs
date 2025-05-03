namespace AuthWithStorage.Domain.Entities
{
    public class Permission : IEntity<Permission>
    {
        public Permission Id { get; set; }
        public string Name { get; set; }
    }
}
