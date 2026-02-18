using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PersonApi.Controllers;
using PersonApi.Models;
using PersonApi.Services;
using Xunit;

namespace PersonApi.Tests.Controllers;

public class CitiesControllerTests
{
    private readonly Mock<ICityService> _mockService;
    private readonly Mock<ILogger<CitiesController>> _mockLogger;
    private readonly CitiesController _controller;

    public CitiesControllerTests()
    {
        _mockService = new Mock<ICityService>();
        _mockLogger = new Mock<ILogger<CitiesController>>();
        _controller = new CitiesController(_mockService.Object, _mockLogger.Object);
    }

    #region GetAllCities Tests

    [Fact]
    public async Task GetAllCities_ReturnsOkWithCities_WhenCitiesExist()
    {
        // Arrange
        var cities = new List<City>
        {
            new City { Id = 1, Name = "Paris", Country = "France" },
            new City { Id = 2, Name = "Tokyo", Country = "Japan" },
            new City { Id = 3, Name = "New York", Country = "USA" }
        };
        _mockService.Setup(s => s.GetAllCitiesAsync()).ReturnsAsync(cities);

        // Act
        var result = await _controller.GetAllCities();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCities = okResult.Value.Should().BeAssignableTo<IEnumerable<City>>().Subject;
        returnedCities.Should().HaveCount(3);
        returnedCities.Should().Contain(c => c.Name == "Paris");
        returnedCities.Should().Contain(c => c.Name == "Tokyo");
        returnedCities.Should().Contain(c => c.Name == "New York");
    }

    [Fact]
    public async Task GetAllCities_ReturnsOkWithEmptyList_WhenNoCitiesExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllCitiesAsync()).ReturnsAsync(new List<City>());

        // Act
        var result = await _controller.GetAllCities();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCities = okResult.Value.Should().BeAssignableTo<IEnumerable<City>>().Subject;
        returnedCities.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCities_CallsServiceGetAllCitiesAsync()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllCitiesAsync()).ReturnsAsync(new List<City>());

        // Act
        await _controller.GetAllCities();

        // Assert
        _mockService.Verify(s => s.GetAllCitiesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllCities_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllCitiesAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllCities();

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Error retrieving cities");
    }

    [Fact]
    public async Task GetAllCities_LogsInformation_OnSuccess()
    {
        // Arrange
        var cities = new List<City>
        {
            new City { Id = 1, Name = "Paris", Country = "France" },
            new City { Id = 2, Name = "Tokyo", Country = "Japan" }
        };
        _mockService.Setup(s => s.GetAllCitiesAsync()).ReturnsAsync(cities);

        // Act
        await _controller.GetAllCities();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieved 2 cities")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllCities_LogsError_WhenExceptionThrown()
    {
        // Arrange
        var exception = new Exception("Database error");
        _mockService.Setup(s => s.GetAllCitiesAsync()).ThrowsAsync(exception);

        // Act
        await _controller.GetAllCities();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error retrieving cities")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region GetCity Tests

    [Fact]
    public async Task GetCity_ReturnsOkWithCity_WhenCityExists()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _mockService.Setup(s => s.GetCityByIdAsync(1)).ReturnsAsync(city);

        // Act
        var result = await _controller.GetCity(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCity = okResult.Value.Should().BeOfType<City>().Subject;
        returnedCity.Id.Should().Be(1);
        returnedCity.Name.Should().Be("Paris");
        returnedCity.Country.Should().Be("France");
    }

    [Fact]
    public async Task GetCity_ReturnsNotFound_WhenCityDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetCityByIdAsync(999)).ReturnsAsync((City?)null);

        // Act
        var result = await _controller.GetCity(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetCity_CallsServiceGetCityByIdAsync()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _mockService.Setup(s => s.GetCityByIdAsync(1)).ReturnsAsync(city);

        // Act
        await _controller.GetCity(1);

        // Assert
        _mockService.Verify(s => s.GetCityByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetCity_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockService.Setup(s => s.GetCityByIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetCity(1);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Error retrieving city");
    }

    [Fact]
    public async Task GetCity_LogsWarning_WhenCityNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetCityByIdAsync(999)).ReturnsAsync((City?)null);

        // Act
        await _controller.GetCity(999);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("City with id 999 not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCity_LogsError_WhenExceptionThrown()
    {
        // Arrange
        var exception = new Exception("Database error");
        _mockService.Setup(s => s.GetCityByIdAsync(1)).ThrowsAsync(exception);

        // Act
        await _controller.GetCity(1);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error retrieving city with id 1")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task GetCity_HandlesInvalidIds_Gracefully(int invalidId)
    {
        // Arrange
        _mockService.Setup(s => s.GetCityByIdAsync(invalidId)).ReturnsAsync((City?)null);

        // Act
        var result = await _controller.GetCity(invalidId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion
}
