namespace AuthWithStorage.Domain.Entities
{
    public class Permission : IEntity<PermissionType>
    {
        public PermissionType Id { get; set; }
        public string Name { get; set; }
    }
}
