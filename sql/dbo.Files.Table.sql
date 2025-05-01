IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Files' AND xtype='U')
BEGIN
    CREATE TABLE dbo.Files (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL,
        Path NVARCHAR(500) NOT NULL,
        ContentType NVARCHAR(100) NOT NULL,
        Type INT NOT NULL,
        Size BIGINT NOT NULL,
        UploadedByUserId INT NOT NULL,
        UploadedAt DATETIME NOT NULL,
        CONSTRAINT FK_Files_UploadedByUser FOREIGN KEY (UploadedByUserId) REFERENCES dbo.Users(Id)
    );
END;