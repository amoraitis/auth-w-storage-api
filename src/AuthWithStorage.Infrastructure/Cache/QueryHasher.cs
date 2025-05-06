using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AuthWithStorage.Domain.Queries;

namespace AuthWithStorage.Infrastructure.Cache
{
    /// <summary>
    /// Generates a hash key for a given query string using SHA256.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    public class QueryHasher<TQuery> where TQuery : SearchQuery
    {
        /// <summary>
        /// Generates a hash key for the given query string.
        /// </summary>
        /// <param name="query">The query string to hash.</param>
        /// <returns>A hash key as a string.</returns>
        public string GetHashKey(TQuery query)
        {
            if (query == null)
            {
                return string.Empty;
            }

            // TODO: Cache JsonSerializerOptions for better performance
            var queryString = JsonSerializer.Serialize(query, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });
            var queryBytes = Encoding.UTF8.GetBytes(queryString);
            var hashBytes = SHA256.HashData(queryBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}