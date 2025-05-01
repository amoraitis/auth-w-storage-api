IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRoles' AND xtype='U')
BEGIN
    CREATE TABLE UserRoles (
        UserId INT NOT NULL,
        RoleId INT NOT NULL,
        CONSTRAINT FK_UserRoles_UserId FOREIGN KEY (UserId) REFERENCES Users(Id),
        CONSTRAINT FK_UserRoles_RoleId FOREIGN KEY (RoleId) REFERENCES Roles(Id),
        PRIMARY KEY (UserId, RoleId)
    );
END;