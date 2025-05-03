namespace AuthWithStorage.Domain
{
    public enum Permission {
        // TODO: Feed
        Read,
        Write,
        Execute,
        Delete
    }
    public enum UserRole
    {
        Unknown = 0,
        Admin,
        User,
        Guest
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