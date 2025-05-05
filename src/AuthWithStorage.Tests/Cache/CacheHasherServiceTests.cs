using System.Text.Json;
using AuthWithStorage.Domain.Queries;
using AuthWithStorage.Infrastructure.Cache;
using Xunit;

namespace AuthWithStorage.Tests.Cache
{
    public class CacheHasherServiceTests
    {
        private class TestQuery : SearchQuery
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [Fact]
        public void GetHashKey_NullQuery_ReturnsEmptyString()
        {
            // Arrange
            var hasher = new QueryHasher<TestQuery>();

            // Act
            var result = hasher.GetHashKey(null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetHashKey_ValidQuery_ReturnsHash()
        {
            // Arrange
            var hasher = new QueryHasher<TestQuery>();
            var query = new TestQuery { Name = "John", Age = 30 };

            // Act
            var result = hasher.GetHashKey(query);

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void GetHashKey_SameQuery_ReturnsSameHash()
        {
            // Arrange
            var hasher = new QueryHasher<TestQuery>();
            var query1 = new TestQuery { Name = "John", Age = 30 };
            var query2 = new TestQuery { Name = "John", Age = 30 };

            // Act
            var hash1 = hasher.GetHashKey(query1);
            var hash2 = hasher.GetHashKey(query2);

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashKey_DifferentQueries_ReturnDifferentHashes()
        {
            // Arrange
            var hasher = new QueryHasher<TestQuery>();
            var query1 = new TestQuery { Name = "John", Age = 30 };
            var query2 = new TestQuery { Name = "Jane", Age = 25 };

            // Act
            var hash1 = hasher.GetHashKey(query1);
            var hash2 = hasher.GetHashKey(query2);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }
    }
}
