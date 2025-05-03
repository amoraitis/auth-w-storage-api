using AuthWithStorage.Domain.Entities;
using AuthWithStorage.Infrastructure.Data;
using Dapper;

namespace AuthWithStorage.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User, int>
    {
        public UserRepository(DbContext context) : base(context)
        { }

        public override async Task<List<User>> GetAllAsync()
        {
            var sql = "SELECT * FROM Users";
            using var connection = _context.CreateConnection();
            return (await connection.QueryAsync<User>(sql)).ToList();
        }

        public override async Task AddAsync(User entity)
        {
            var sql = "INSERT INTO Users (Username, Email, PasswordHash, CreatedAt) VALUES (@Username, @Email, @PasswordHash, @CreatedAt)";
            await _context.CreateConnection().ExecuteAsync(sql, new
            {
                entity.Username,
                entity.Email,
                entity.PasswordHash,
                entity.CreatedAt
            });
        }

        public override async Task UpdateAsync(User entity)
        {
            var sql = "UPDATE Users SET Username = @Username, Email = @Email, PasswordHash = @PasswordHash, UpdatedAt = @UpdatedAt WHERE Id = @Id";
         
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(sql, new
            {
                entity.Username,
                entity.Email,
                entity.PasswordHash,
                entity.UpdatedAt,
                entity.Id
            });
        }

        public override string TableName => "Users";
    }
}
