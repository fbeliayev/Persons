using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PersonApi.Data;
using PersonApi.Models;
using PersonApi.Services;
using Xunit;

namespace PersonApi.Tests.Services;

public class CityServiceTests : IDisposable
{
    private readonly PersonDbContext _context;
    private readonly Mock<ILogger<CityService>> _mockLogger;
    private readonly CityService _service;

    public CityServiceTests()
    {
        var options = new DbContextOptionsBuilder<PersonDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PersonDbContext(options);
        _mockLogger = new Mock<ILogger<CityService>>();
        _service = new CityService(_context, _mockLogger.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var cities = new List<City>
        {
            new City { Id = 1, Name = "Paris", Country = "France" },
            new City { Id = 2, Name = "Tokyo", Country = "Japan" },
            new City { Id = 3, Name = "New York", Country = "USA" }
        };

        _context.Cities.AddRange(cities);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region GetAllCitiesAsync Tests

    [Fact]
    public async Task GetAllCitiesAsync_ReturnsAllCities()
    {
        // Act
        var result = await _service.GetAllCitiesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Name == "Paris");
        result.Should().Contain(c => c.Name == "Tokyo");
        result.Should().Contain(c => c.Name == "New York");
    }

    [Fact]
    public async Task GetAllCitiesAsync_ReturnsEmptyList_WhenNoCitiesExist()
    {
        // Arrange
        _context.Cities.RemoveRange(_context.Cities);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllCitiesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCitiesAsync_LogsInformation()
    {
        // Act
        await _service.GetAllCitiesAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Getting all cities")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region GetCityByIdAsync Tests

    [Fact]
    public async Task GetCityByIdAsync_ReturnsCity_WhenCityExists()
    {
        // Act
        var result = await _service.GetCityByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Paris");
        result.Country.Should().Be("France");
    }

    [Fact]
    public async Task GetCityByIdAsync_ReturnsNull_WhenCityDoesNotExist()
    {
        // Act
        var result = await _service.GetCityByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCityByIdAsync_LogsInformation()
    {
        // Act
        await _service.GetCityByIdAsync(1);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Getting city with id 1")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task GetCityByIdAsync_ReturnsNull_WhenIdIsInvalid(int invalidId)
    {
        // Act
        var result = await _service.GetCityByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCityByIdAsync_WorksCorrectly_ForMultipleCalls()
    {
        // Act
        var result1 = await _service.GetCityByIdAsync(1);
        var result2 = await _service.GetCityByIdAsync(2);
        var result3 = await _service.GetCityByIdAsync(3);

        // Assert
        result1!.Name.Should().Be("Paris");
        result2!.Name.Should().Be("Tokyo");
        result3!.Name.Should().Be("New York");
    }

    #endregion
}
