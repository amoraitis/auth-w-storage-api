using AuthWithStorage.Domain;
using AuthWithStorage.Domain.Entities;
using AuthWithStorage.Domain.Queries;
using AuthWithStorage.Infrastructure.Data;
using Dapper;

namespace AuthWithStorage.Infrastructure.Repositories
{
    public class FileRepository : BaseRepository<FileModel, int, FileSearchQuery>
    {
        public FileRepository(DbContext context) : base(context)
        { }

        public override async Task<List<FileModel>> GetAllAsync(FileSearchQuery searchQuery)
        {
            var sql = "EXEC dbo.GetFilesFiltered @Filename, @FileType, @UploadedByUserId, @SortBy, @SortOrder, @Offset, @PageSize";
            using var connection = Context.CreateConnection();
            var result = await connection.QueryAsync<dynamic>(sql, new
            {
                Filename = searchQuery.Name,
                FileType = searchQuery.Type,
                UploadedByUserId = searchQuery.UploadedByUserId,
                SortBy = searchQuery.SortBy ?? "Name",
                SortOrder = searchQuery.SortOrder ?? "ASC",
                Offset = (searchQuery.Page - 1) * searchQuery.PageSize,
                searchQuery.PageSize
            });

            return result.Select(r => new FileModel
            {
                Id = r.Id,
                Name = r.Name,
                Path = r.Path,
                ContentType = r.ContentType,
                UploadedAt = r.UploadedAt,
                Type = (FileType)Enum.Parse(typeof(FileType), r.Type.ToString()),
                UpdatedAt = r.UpdatedAt,
                Size = r.Size,
                UploadedByUserId = r.UploadedByUserId,
                UploadedByUsername = r.UploadedByUsername
            }).ToList();
        }

        public override async Task<int> AddAsync(FileModel entity)
        {
            var sql = @"
            INSERT INTO Files (Name, Path, ContentType, Type, Size, UploadedByUserId, UploadedAt, UpdatedAt) 
            OUTPUT INSERTED.Id
            VALUES (@Name, @Path, @ContentType, @Type, @Size, @UploadedByUserId, @UploadedAt, @UpdatedAt)";
            
            var insertedId = await Context.CreateConnection().ExecuteScalarAsync<int>(sql, new
            {
            entity.Name,
            entity.Path,
            entity.ContentType,
            Type = (int)entity.Type,
            entity.Size,
            entity.UploadedByUserId,
            entity.UploadedAt,
            entity.UpdatedAt
            });

            return insertedId;
        }

        public override async Task UpdateAsync(FileModel entity)
        {
            var sql = @"
                UPDATE Files
                SET Name = @Name,
                    Path = @Path,
                    ContentType = @ContentType,
                    Type = @Type,
                    Size = @Size,
                    UploadedByUserId = @UploadedByUserId,
                    UploadedAt = @UploadedAt,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            await Context.CreateConnection().ExecuteAsync(sql, new
            {
                entity.Id,
                entity.Name,
                entity.Path,
                entity.ContentType,
                Type = (int)entity.Type,
                entity.Size,
                entity.UploadedByUserId,
                entity.UploadedAt,
                entity.UpdatedAt
            });
        }

        public override string TableName => "Files";
    }
}
