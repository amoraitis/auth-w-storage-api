﻿namespace AuthWithStorage.Infrastructure.Storage
{
    public class BlobStorageSettings
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}
