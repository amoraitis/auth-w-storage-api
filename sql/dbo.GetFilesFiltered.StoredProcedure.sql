IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.GetFilesFiltered') AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].[GetFilesFiltered];
GO

CREATE PROCEDURE [dbo].[GetFilesFiltered]
    @Filename NVARCHAR(255) = NULL,
    @FileType NVARCHAR(50) = NULL,
    @UploadedByUserId INT = NULL,
    @SortBy NVARCHAR(50) = 'Name',
    @SortOrder NVARCHAR(4) = 'ASC',
    @Offset INT = 0,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        f.Id, 
        f.Name,
        f.Path,
        f.ContentType, 
        f.UploadedAt,
        f.Type,
        f.UpdatedAt AS UpdatedAt,
        f.Size,
        f.UploadedByUserId,
        u.Username AS UploadedByUsername
    FROM 
        Files f
    INNER JOIN 
        Users u ON f.UploadedByUserId = u.Id
    WHERE 
        (@Filename IS NULL OR f.Name LIKE '%' + @Filename + '%')
        AND (@FileType IS NULL OR f.ContentType = @FileType)
        AND (@UploadedByUserId IS NULL OR f.UploadedByUserId = @UploadedByUserId)
    ORDER BY 
        CASE WHEN @SortBy = 'Name' AND @SortOrder = 'ASC' THEN f.Name END ASC,
        CASE WHEN @SortBy = 'Name' AND @SortOrder = 'DESC' THEN f.Name END DESC,
        CASE WHEN @SortBy = 'UploadedAt' AND @SortOrder = 'ASC' THEN f.UploadedAt END ASC,
        CASE WHEN @SortBy = 'UploadedAt' AND @SortOrder = 'DESC' THEN f.UploadedAt END DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
