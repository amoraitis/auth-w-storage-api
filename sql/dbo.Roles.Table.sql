IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Roles' AND xtype='U')
BEGIN
    CREATE TABLE Roles (
        Id INT PRIMARY KEY NOT NULL,
        Name NVARCHAR(255) NOT NULL
    );
END;