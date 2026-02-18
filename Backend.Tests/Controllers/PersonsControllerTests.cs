using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PersonApi.Controllers;
using PersonApi.Models;
using PersonApi.Services;
using Xunit;

namespace PersonApi.Tests.Controllers;

public class PersonsControllerTests
{
    private readonly Mock<IPersonService> _mockService;
    private readonly Mock<ILogger<PersonsController>> _mockLogger;
    private readonly PersonsController _controller;

    public PersonsControllerTests()
    {
        _mockService = new Mock<IPersonService>();
        _mockLogger = new Mock<ILogger<PersonsController>>();
        _controller = new PersonsController(_mockService.Object, _mockLogger.Object);
    }

    #region GetAllPersons Tests

    [Fact]
    public async Task GetAllPersons_ReturnsOkWithEmptyList_WhenNoPersonsExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllPersonsAsync())
            .ReturnsAsync(new List<Person>());

        // Act
        var result = await _controller.GetAllPersons();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var persons = okResult.Value.Should().BeAssignableTo<IEnumerable<Person>>().Subject;
        persons.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllPersons_ReturnsOkWithPersons_WhenPersonsExist()
    {
        // Arrange
        var persons = new List<Person>
        {
            new Person { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 },
            new Person { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 }
        };
        _mockService.Setup(s => s.GetAllPersonsAsync()).ReturnsAsync(persons);

        // Act
        var result = await _controller.GetAllPersons();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPersons = okResult.Value.Should().BeAssignableTo<IEnumerable<Person>>().Subject;
        returnedPersons.Should().HaveCount(2);
        returnedPersons.Should().Contain(p => p.FirstName == "John");
        returnedPersons.Should().Contain(p => p.FirstName == "Jane");
    }

    [Fact]
    public async Task GetAllPersons_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllPersonsAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllPersons();

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region GetPerson Tests

    [Fact]
    public async Task GetPerson_ReturnsOkWithPerson_WhenPersonExists()
    {
        // Arrange
        var person = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };
        _mockService.Setup(s => s.GetPersonByIdAsync(1)).ReturnsAsync(person);

        // Act
        var result = await _controller.GetPerson(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPerson = okResult.Value.Should().BeOfType<Person>().Subject;
        returnedPerson.Id.Should().Be(1);
        returnedPerson.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetPerson_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetPersonByIdAsync(999)).ReturnsAsync((Person?)null);

        // Act
        var result = await _controller.GetPerson(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetPerson_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockService.Setup(s => s.GetPersonByIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetPerson(1);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region CreatePerson Tests

    [Fact]
    public async Task CreatePerson_ReturnsCreatedAtAction_WhenPersonIsValid()
    {
        // Arrange
        var newPerson = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };
        var createdPerson = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };
        _mockService.Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(createdPerson);

        // Act
        var result = await _controller.CreatePerson(newPerson);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(PersonsController.GetPerson));
        createdResult.RouteValues!["id"].Should().Be(1);
        var returnedPerson = createdResult.Value.Should().BeOfType<Person>().Subject;
        returnedPerson.Id.Should().Be(1);
        returnedPerson.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task CreatePerson_CallsServiceWithCorrectPerson()
    {
        // Arrange
        var newPerson = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };
        var createdPerson = new Person { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        _mockService.Setup(s => s.CreatePersonAsync(It.IsAny<Person>())).ReturnsAsync(createdPerson);

        // Act
        await _controller.CreatePerson(newPerson);

        // Assert
        _mockService.Verify(s => s.CreatePersonAsync(It.Is<Person>(p =>
            p.FirstName == "John" &&
            p.LastName == "Doe" &&
            p.Email == "john@test.com" &&
            p.Age == 30
        )), Times.Once);
    }

    [Fact]
    public async Task CreatePerson_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var newPerson = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        _mockService.Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreatePerson(newPerson);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region UpdatePerson Tests

    [Fact]
    public async Task UpdatePerson_ReturnsOkWithUpdatedPerson_WhenPersonExists()
    {
        // Arrange
        var updatedPerson = new Person
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            Age = 25
        };
        var resultPerson = new Person
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            Age = 25
        };
        _mockService.Setup(s => s.UpdatePersonAsync(1, It.IsAny<Person>()))
            .ReturnsAsync(resultPerson);

        // Act
        var result = await _controller.UpdatePerson(1, updatedPerson);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPerson = okResult.Value.Should().BeOfType<Person>().Subject;
        returnedPerson.Id.Should().Be(1);
        returnedPerson.FirstName.Should().Be("Jane");
    }

    [Fact]
    public async Task UpdatePerson_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        var updatedPerson = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };
        _mockService.Setup(s => s.UpdatePersonAsync(999, It.IsAny<Person>()))
            .ReturnsAsync((Person?)null);

        // Act
        var result = await _controller.UpdatePerson(999, updatedPerson);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdatePerson_CallsServiceWithCorrectIdAndPerson()
    {
        // Arrange
        var updatedPerson = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };
        var resultPerson = new Person { Id = 1, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };
        _mockService.Setup(s => s.UpdatePersonAsync(1, It.IsAny<Person>())).ReturnsAsync(resultPerson);

        // Act
        await _controller.UpdatePerson(1, updatedPerson);

        // Assert
        _mockService.Verify(s => s.UpdatePersonAsync(1, It.Is<Person>(p =>
            p.FirstName == "Jane" &&
            p.LastName == "Smith" &&
            p.Email == "jane@test.com" &&
            p.Age == 25
        )), Times.Once);
    }

    [Fact]
    public async Task UpdatePerson_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var updatedPerson = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };
        _mockService.Setup(s => s.UpdatePersonAsync(It.IsAny<int>(), It.IsAny<Person>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdatePerson(1, updatedPerson);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region DeletePerson Tests

    [Fact]
    public async Task DeletePerson_ReturnsNoContent_WhenPersonExists()
    {
        // Arrange
        _mockService.Setup(s => s.DeletePersonAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeletePerson(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeletePerson_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.DeletePersonAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeletePerson(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeletePerson_CallsServiceWithCorrectId()
    {
        // Arrange
        _mockService.Setup(s => s.DeletePersonAsync(1)).ReturnsAsync(true);

        // Act
        await _controller.DeletePerson(1);

        // Assert
        _mockService.Verify(s => s.DeletePersonAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeletePerson_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockService.Setup(s => s.DeletePersonAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeletePerson(1);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public async Task CreatePerson_WithEmptyStrings_CallsService()
    {
        // Arrange
        var person = new Person { FirstName = "", LastName = "", Email = "", Age = 0 };
        var createdPerson = new Person { Id = 1, FirstName = "", LastName = "", Email = "", Age = 0 };
        _mockService.Setup(s => s.CreatePersonAsync(It.IsAny<Person>())).ReturnsAsync(createdPerson);

        // Act
        var result = await _controller.CreatePerson(person);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        _mockService.Verify(s => s.CreatePersonAsync(It.IsAny<Person>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePerson_WithNegativeAge_CallsService()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = -5 };
        var updatedPerson = new Person { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = -5 };
        _mockService.Setup(s => s.UpdatePersonAsync(1, It.IsAny<Person>())).ReturnsAsync(updatedPerson);

        // Act
        var result = await _controller.UpdatePerson(1, person);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region GetPersonCities Tests

    [Fact]
    public async Task GetPersonCities_ReturnsOkWithCities_WhenPersonExists()
    {
        // Arrange
        var personCities = new List<PersonCity>
        {
            new PersonCity { PersonId = 1, CityId = 1, City = new City { Id = 1, Name = "Paris", Country = "France" }, IsVisited = false },
            new PersonCity { PersonId = 1, CityId = 2, City = new City { Id = 2, Name = "Tokyo", Country = "Japan" }, IsVisited = true, VisitedDate = DateTime.UtcNow }
        };
        _mockService.Setup(s => s.GetPersonByIdAsync(1)).ReturnsAsync(new Person { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 });
        _mockService.Setup(s => s.GetPersonCitiesAsync(1)).ReturnsAsync(personCities);

        // Act
        var result = await _controller.GetPersonCities(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCities = okResult.Value.Should().BeAssignableTo<IEnumerable<PersonCity>>().Subject;
        returnedCities.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPersonCities_ReturnsOkWithEmptyList_WhenPersonHasNoCities()
    {
        // Arrange
        _mockService.Setup(s => s.GetPersonByIdAsync(1)).ReturnsAsync(new Person { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 });
        _mockService.Setup(s => s.GetPersonCitiesAsync(1)).ReturnsAsync(new List<PersonCity>());

        // Act
        var result = await _controller.GetPersonCities(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCities = okResult.Value.Should().BeAssignableTo<IEnumerable<PersonCity>>().Subject;
        returnedCities.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPersonCities_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetPersonByIdAsync(999)).ReturnsAsync((Person?)null);

        // Act
        var result = await _controller.GetPersonCities(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetPersonCities_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockService.Setup(s => s.GetPersonByIdAsync(1)).ReturnsAsync(new Person { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 });
        _mockService.Setup(s => s.GetPersonCitiesAsync(1)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetPersonCities(1);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region AddCityToPerson Tests

    [Fact]
    public async Task AddCityToPerson_ReturnsOkWithPersonCity_WhenSuccessful()
    {
        // Arrange
        var personCity = new PersonCity
        {
            PersonId = 1,
            CityId = 1,
            City = new City { Id = 1, Name = "Paris", Country = "France" },
            IsVisited = false
        };
        _mockService.Setup(s => s.AddCityToPersonAsync(1, 1)).ReturnsAsync(personCity);

        // Act
        var result = await _controller.AddCityToPerson(1, 1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPersonCity = okResult.Value.Should().BeOfType<PersonCity>().Subject;
        returnedPersonCity.PersonId.Should().Be(1);
        returnedPersonCity.CityId.Should().Be(1);
        returnedPersonCity.IsVisited.Should().BeFalse();
    }

    [Fact]
    public async Task AddCityToPerson_ReturnsNotFound_WhenPersonOrCityDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.AddCityToPersonAsync(999, 1)).ReturnsAsync((PersonCity?)null);

        // Act
        var result = await _controller.AddCityToPerson(999, 1);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be("Person or city not found");
    }

    [Fact]
    public async Task AddCityToPerson_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var personCity = new PersonCity
        {
            PersonId = 1,
            CityId = 2,
            City = new City { Id = 2, Name = "Tokyo", Country = "Japan" },
            IsVisited = false
        };
        _mockService.Setup(s => s.AddCityToPersonAsync(1, 2)).ReturnsAsync(personCity);

        // Act
        await _controller.AddCityToPerson(1, 2);

        // Assert
        _mockService.Verify(s => s.AddCityToPersonAsync(1, 2), Times.Once);
    }

    [Fact]
    public async Task AddCityToPerson_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockService.Setup(s => s.AddCityToPersonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.AddCityToPerson(1, 1);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region UpdatePersonCity Tests

    [Fact]
    public async Task UpdatePersonCity_ReturnsOkWithUpdatedPersonCity_WhenSuccessful()
    {
        // Arrange
        var personCity = new PersonCity
        {
            PersonId = 1,
            CityId = 1,
            City = new City { Id = 1, Name = "Paris", Country = "France" },
            IsVisited = true,
            VisitedDate = DateTime.UtcNow
        };
        var request = new UpdatePersonCityRequest { IsVisited = true, VisitedDate = DateTime.UtcNow };
        _mockService.Setup(s => s.MarkCityAsVisitedAsync(1, 1, true, It.IsAny<DateTime?>())).ReturnsAsync(personCity);

        // Act
        var result = await _controller.UpdatePersonCity(1, 1, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPersonCity = okResult.Value.Should().BeOfType<PersonCity>().Subject;
        returnedPersonCity.IsVisited.Should().BeTrue();
        returnedPersonCity.VisitedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatePersonCity_ReturnsNotFound_WhenPersonCityDoesNotExist()
    {
        // Arrange
        var request = new UpdatePersonCityRequest { IsVisited = true };
        _mockService.Setup(s => s.MarkCityAsVisitedAsync(999, 1, true, null)).ReturnsAsync((PersonCity?)null);

        // Act
        var result = await _controller.UpdatePersonCity(999, 1, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdatePersonCity_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var visitedDate = new DateTime(2023, 6, 15);
        var request = new UpdatePersonCityRequest { IsVisited = true, VisitedDate = visitedDate };
        var personCity = new PersonCity
        {
            PersonId = 1,
            CityId = 1,
            City = new City { Id = 1, Name = "Paris", Country = "France" },
            IsVisited = true,
            VisitedDate = visitedDate
        };
        _mockService.Setup(s => s.MarkCityAsVisitedAsync(1, 1, true, visitedDate)).ReturnsAsync(personCity);

        // Act
        await _controller.UpdatePersonCity(1, 1, request);

        // Assert
        _mockService.Verify(s => s.MarkCityAsVisitedAsync(1, 1, true, visitedDate), Times.Once);
    }

    [Fact]
    public async Task UpdatePersonCity_CanUnmarkAsVisited()
    {
        // Arrange
        var personCity = new PersonCity
        {
            PersonId = 1,
            CityId = 1,
            City = new City { Id = 1, Name = "Paris", Country = "France" },
            IsVisited = false,
            VisitedDate = null
        };
        var request = new UpdatePersonCityRequest { IsVisited = false };
        _mockService.Setup(s => s.MarkCityAsVisitedAsync(1, 1, false, null)).ReturnsAsync(personCity);

        // Act
        var result = await _controller.UpdatePersonCity(1, 1, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPersonCity = okResult.Value.Should().BeOfType<PersonCity>().Subject;
        returnedPersonCity.IsVisited.Should().BeFalse();
        returnedPersonCity.VisitedDate.Should().BeNull();
    }

    [Fact]
    public async Task UpdatePersonCity_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var request = new UpdatePersonCityRequest { IsVisited = true };
        _mockService.Setup(s => s.MarkCityAsVisitedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<DateTime?>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdatePersonCity(1, 1, request);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region RemoveCityFromPerson Tests

    [Fact]
    public async Task RemoveCityFromPerson_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        _mockService.Setup(s => s.RemoveCityFromPersonAsync(1, 1)).ReturnsAsync(true);

        // Act
        var result = await _controller.RemoveCityFromPerson(1, 1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task RemoveCityFromPerson_ReturnsNotFound_WhenPersonCityDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.RemoveCityFromPersonAsync(999, 1)).ReturnsAsync(false);

        // Act
        var result = await _controller.RemoveCityFromPerson(999, 1);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task RemoveCityFromPerson_CallsServiceWithCorrectParameters()
    {
        // Arrange
        _mockService.Setup(s => s.RemoveCityFromPersonAsync(1, 2)).ReturnsAsync(true);

        // Act
        await _controller.RemoveCityFromPerson(1, 2);

        // Assert
        _mockService.Verify(s => s.RemoveCityFromPersonAsync(1, 2), Times.Once);
    }

    [Fact]
    public async Task RemoveCityFromPerson_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockService.Setup(s => s.RemoveCityFromPersonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.RemoveCityFromPerson(1, 1);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion
}
