using System.Text.Json;
using System.Threading.Tasks;
using AuthWithStorage.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AuthWithStorage.Tests.Cache
{
    public class CachingServiceTests
    {
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly CachingService _cachingService;
        private readonly CacheOptions _cacheOptions;

        public CachingServiceTests()
        {
            _cacheMock = new Mock<IDistributedCache>();
            _cacheOptions = new CacheOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
            var optionsMock = new Mock<IOptions<CacheOptions>>();
            optionsMock.Setup(o => o.Value).Returns(_cacheOptions);

            _cachingService = new CachingService(_cacheMock.Object, optionsMock.Object);
        }

        [Fact]
        public async Task GetOrSetAsync_ReturnsCachedData_WhenCacheHit()
        {
            // Arrange
            var key = "test-key";
            var expectedData = new TestData { Id = 1, Name = "Cached Data" };
            var cachedData = JsonSerializer.SerializeToUtf8Bytes(expectedData);

            _cacheMock.Setup(c => c.GetAsync(key, default)).ReturnsAsync(cachedData);

            // Act
            var result = await _cachingService.GetOrSetAsync<TestData>(key, () => Task.FromResult(new TestData()));

            // Assert
            Assert.Equal(expectedData.Id, result.Id);
            Assert.Equal(expectedData.Name, result.Name);
            _cacheMock.Verify(c => c.GetAsync(key, default), Times.Once);
            _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Never);
        }

        [Fact]
        public async Task GetOrSetAsync_CachesAndReturnsData_WhenCacheMiss()
        {
            // Arrange
            var key = "test-key";
            var expectedData = new TestData { Id = 2, Name = "Fetched Data" };

            _cacheMock.Setup(c => c.GetAsync(key, default)).ReturnsAsync((byte[])null);

            // Act
            var result = await _cachingService.GetOrSetAsync<TestData>(key, () => Task.FromResult(expectedData));

            // Assert
            Assert.Equal(expectedData.Id, result.Id);
            Assert.Equal(expectedData.Name, result.Name);
            _cacheMock.Verify(c => c.GetAsync(key, default), Times.Once);
            _cacheMock.Verify(c => c.SetAsync(
                key,
                It.Is<byte[]>(b => JsonSerializer.Deserialize<TestData>(b, JsonSerializerOptions.Default).Id == expectedData.Id),
                It.Is<DistributedCacheEntryOptions>(o => o.SlidingExpiration == _cacheOptions.SlidingExpiration),
                default), Times.Once);
        }

        private class TestData
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
