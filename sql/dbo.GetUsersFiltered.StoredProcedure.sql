IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.GetUsersFiltered') AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].[GetUsersFiltered];
GO

CREATE PROCEDURE [dbo].[GetUsersFiltered]
    @Username NVARCHAR(255) = NULL,
    @Role NVARCHAR(255) = NULL,
    @PermissionName NVARCHAR(255) = NULL,
    @SortBy NVARCHAR(50) = 'Username',
    @SortOrder NVARCHAR(4) = 'ASC',
    @Offset INT = 0,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.Id, 
        u.Username, 
        u.Email, 
        u.CreatedAt,
        u.UpdatedAt,
        u.PasswordHash,
        r.Name AS Role,
        STRING_AGG(p.Id, ',') AS PermissionIds
    FROM 
        Users u
    INNER JOIN 
        UserRoles ur ON u.Id = ur.UserId
    INNER JOIN 
        Roles r ON ur.RoleId = r.Id
    LEFT JOIN 
        RolePermissions rp ON r.Id = rp.RoleId
    LEFT JOIN 
        Permissions p ON rp.PermissionId = p.Id
    WHERE 
        (@Username IS NULL OR u.Username LIKE '%' + @Username + '%')
        AND (@Role IS NULL OR r.Name = @Role)
        AND (@PermissionName IS NULL OR p.Name = @PermissionName)
    GROUP BY 
        u.Id, u.Username, u.Email, u.CreatedAt, u.UpdatedAt, u.PasswordHash, r.Name
    ORDER BY 
        CASE WHEN @SortBy = 'Username' AND @SortOrder = 'ASC' THEN u.Username END ASC,
        CASE WHEN @SortBy = 'Username' AND @SortOrder = 'DESC' THEN u.Username END DESC,
        CASE WHEN @SortBy = 'Email' AND @SortOrder = 'ASC' THEN u.Email END ASC,
        CASE WHEN @SortBy = 'Email' AND @SortOrder = 'DESC' THEN u.Email END DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;