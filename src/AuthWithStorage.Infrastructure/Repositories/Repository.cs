using AuthWithStorage.Domain.Entities;
using AuthWithStorage.Infrastructure.Data;
using Dapper;

namespace AuthWithStorage.Infrastructure.Repositories
{
    public interface IRepository<T, TKey> where T : IEntity<TKey>
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }

    public abstract class BaseRepository<T, TKey> : IRepository<T, TKey> where T : IEntity<TKey>
    {
        protected readonly DbContext _context;

        protected BaseRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await _context.CreateConnection()
                .QuerySingleOrDefaultAsync<T>(
                    $"SELECT TOP(1) * FROM {this.TableName} WHERE Id = @Id",
                    new { Id = id });
        }

        public abstract Task<List<T>> GetAllAsync();

        public abstract Task AddAsync(T entity);
        public abstract Task UpdateAsync(T entity);

        public async Task DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            await _context.CreateConnection()
                .ExecuteAsync($"DELETE FROM {this.TableName} WHERE Id = @Id", new { Id = id });
        }

        public abstract string TableName { get; }
    }
}
