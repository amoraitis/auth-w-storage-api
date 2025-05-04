namespace AuthWithStorage.Domain
{
    public enum PermissionType {
        // TODO: Feed
        Unknown = 0,
        Read,
        Write,
        Delete,
        Update
    }
    public enum UserRole
    {
        Unknown = 0,
        Admin = 1,
        User = 2,
        Manager = 3
    }

    public enum FileType
    {
        Unknown = 0,
        Image,
        Document,
        Video,
        Audio,
        Other
    }

    public enum StorageType
    {
        Unknown = 0,
        Local,
        Cloud
    }
    
}