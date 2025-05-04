-- Seed Roles
IF NOT EXISTS (SELECT 1 FROM Roles)
BEGIN
    INSERT INTO Roles (Id, Name)
    VALUES 
        (1, 'Admin'),
        (2, 'User'),
        (3, 'Manager');
END;

-- Seed Users
IF NOT EXISTS (SELECT 1 FROM Users)
BEGIN
    INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
    VALUES 
        -- Passes: emkuQ3>!oVNq
        ('admin', 'admin@example.com', 'vzf/1r5mR0XIZxBzHmufMSoZUfieqowP6zK+Yg9JmrjlUYHYgWFf7kdOZ9hXdo9wQWXIR4ljgMp/8VqLYzF8BQ==:RI0MQ+84tyKnRJ+9c55bKiZdUUX2gtHn4XWDpLR0KIg=', GETDATE()),
        ('user1', 'user1@example.com', 'vzf/1r5mR0XIZxBzHmufMSoZUfieqowP6zK+Yg9JmrjlUYHYgWFf7kdOZ9hXdo9wQWXIR4ljgMp/8VqLYzF8BQ==:RI0MQ+84tyKnRJ+9c55bKiZdUUX2gtHn4XWDpLR0KIg=', GETDATE()),
        ('manager', 'manager@example.com', 'vzf/1r5mR0XIZxBzHmufMSoZUfieqowP6zK+Yg9JmrjlUYHYgWFf7kdOZ9hXdo9wQWXIR4ljgMp/8VqLYzF8BQ==:RI0MQ+84tyKnRJ+9c55bKiZdUUX2gtHn4XWDpLR0KIg=', GETDATE());
END;

-- Seed UserRoles
IF NOT EXISTS (SELECT 1 FROM UserRoles)
BEGIN
    INSERT INTO UserRoles (UserId, RoleId)
    VALUES 
        (1, 1), -- Admin
        (2, 2), -- User
        (3, 3); -- Manager
END;

-- Seed Permissions
IF NOT EXISTS (SELECT 1 FROM Permissions)
BEGIN
    INSERT INTO Permissions (Id, Name)
    VALUES 
        (1, 'Read'),
        (2, 'Write'),
        (3, 'Delete'),
        (4, 'Update');
END;

-- Seed RolePermissions
IF NOT EXISTS (SELECT 1 FROM RolePermissions)
BEGIN
    INSERT INTO RolePermissions (RoleId, PermissionId)
    VALUES 
        (1, 1), -- Admin - Read
        (1, 2), -- Admin - Write
        (1, 3), -- Admin - Delete
        (1, 4), -- Admin - Update
        (2, 1), -- User - Read
        (3, 1), -- Manager - Read
        (3, 2) -- Manager - Write;
END;