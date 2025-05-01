IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AuditLogs' AND xtype='U')
BEGIN
    CREATE TABLE AuditLogs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Action NVARCHAR(255) NOT NULL,
        Entity NVARCHAR(255) NOT NULL,
        EntityId NVARCHAR(255) NOT NULL,
        Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
        Metadata NVARCHAR(MAX) NULL,
        CONSTRAINT FK_AuditLog_User FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END;