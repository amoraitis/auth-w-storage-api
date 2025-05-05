using AuthWithStorage.Domain.Entities;
using AuthWithStorage.Domain.Queries;
using AuthWithStorage.Infrastructure.Data;
using Dapper;

namespace AuthWithStorage.Infrastructure.Repositories
{
    /// <summary>
    /// Defines a generic repository interface for performing CRUD operations.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the entity's key.</typeparam>
    /// <typeparam name="TSearch">The type of the search query.</typeparam>
    public interface IRepository<T, TKey, in TSearch>
        where T : IEntity<TKey>
        where TSearch : SearchQuery
    {
        /// <summary>
        /// Retrieves an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all entities that match the specified search query.
        /// </summary>
        /// <param name="searchQuery">The search query to filter entities.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of entities.</returns>
        Task<List<T>> GetAllAsync(TSearch searchQuery);

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<TKey> AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteAsync(int id);
    }

    /// <inheritdoc cref="IRepository{T,TKey,TSearch}"/>
    public abstract class BaseRepository<T, TKey, TSearch> : IRepository<T, TKey, TSearch>
            where T : IEntity<TKey>
            where TSearch : SearchQuery
    {
        protected readonly DbContext Context;

        protected BaseRepository(DbContext context)
        {
            Context = context;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            using var connection = Context.CreateConnection();
            return await Context.CreateConnection()
                .QuerySingleOrDefaultAsync<T>(
                    $"SELECT TOP(1) * FROM {this.TableName} WHERE Id = @Id",
                    new { Id = id });
        }

        public abstract Task<List<T>> GetAllAsync(TSearch searchQuery);

        public abstract Task<TKey> AddAsync(T entity);
        public abstract Task UpdateAsync(T entity);

        public async Task DeleteAsync(int id)
        {
            using var connection = Context.CreateConnection();
            await Context.CreateConnection()
                .ExecuteAsync($"DELETE FROM {this.TableName} WHERE Id = @Id", new { Id = id });
        }

        public abstract string TableName { get; }
    }
}
