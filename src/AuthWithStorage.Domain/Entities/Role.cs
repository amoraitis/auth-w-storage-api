using AuthWithStorage.Domain.Enums;

namespace AuthWithStorage.Domain.Entities
{
    public class Role
    {
        public UserRole Id { get; set; }
        public string Name { get; set; }
    }
}
