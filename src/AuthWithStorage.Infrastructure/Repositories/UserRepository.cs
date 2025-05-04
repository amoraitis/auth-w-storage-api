using AuthWithStorage.Domain;
using AuthWithStorage.Domain.Entities;
using AuthWithStorage.Domain.Queries;
using AuthWithStorage.Infrastructure.Data;
using Dapper;

namespace AuthWithStorage.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User, int, UserSearchQuery>
    {
        public UserRepository(DbContext context) : base(context)
        { }

        public override async Task<List<User>> GetAllAsync(UserSearchQuery searchQuery)
        {
            var sql = "EXEC dbo.GetUsersFiltered @Username, @Role, @PermissionName, @SortBy, @SortOrder, @Offset, @PageSize";
            using var connection = Context.CreateConnection();
            var result = await connection.QueryAsync<dynamic>(sql, new
            {
                Username = searchQuery.Name,
                searchQuery.Role,
                PermissionName = searchQuery.Permission,
                SortBy = searchQuery.SortBy ?? "Username",
                SortOrder = searchQuery.SortOrder ?? "ASC",
                Offset = (searchQuery.Page - 1) * searchQuery.PageSize,
                searchQuery.PageSize
            });

            return result.Select(r => new User
            {
                Id = r.Id,
                Username = r.Username,
                Email = r.Email,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                PasswordHash = r.PasswordHash,
                Role = r.Role,
                Permissions = (r.PermissionIds as string)?.Split(',').Select(perm => (PermissionType) int.Parse(perm as string)).ToArray()
            }).ToList();
        }

        public override async Task AddAsync(User entity)
        {
            var sql = "INSERT INTO Users (Username, Email, PasswordHash, CreatedAt) VALUES (@Username, @Email, @PasswordHash, @CreatedAt)";
            await Context.CreateConnection().ExecuteAsync(sql, new
            {
                entity.Username,
                entity.Email,
                entity.PasswordHash,
                CreatedAt = DateTime.UtcNow
            });
        }

        public override async Task UpdateAsync(User entity)
        {
            var sql = "UPDATE Users SET Username = @Username, Email = @Email, PasswordHash = @PasswordHash, UpdatedAt = @UpdatedAt WHERE Id = @Id";
         
            using var connection = Context.CreateConnection();

            await connection.ExecuteAsync(sql, new
            {
                entity.Username,
                entity.Email,
                entity.PasswordHash,
                UpdatedAt = DateTime.UtcNow,
                entity.Id
            });
        }

        public override string TableName => "Users";
    }
}
