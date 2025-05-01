IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Permissions' AND xtype='U')
CREATE TABLE dbo.Permissions (
    Id INT PRIMARY KEY NOT NULL,
    Name NVARCHAR(255) NOT NULL
);