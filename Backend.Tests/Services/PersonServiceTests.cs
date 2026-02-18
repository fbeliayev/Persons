using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PersonApi.Data;
using PersonApi.Models;
using PersonApi.Services;
using Xunit;

namespace PersonApi.Tests.Services;

public class PersonServiceTests : IDisposable
{
    private readonly PersonDbContext _context;
    private readonly PersonService _service;

    public PersonServiceTests()
    {
        var options = new DbContextOptionsBuilder<PersonDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PersonDbContext(options);
        _service = new PersonService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region GetAllPersonsAsync Tests

    [Fact]
    public async Task GetAllPersonsAsync_WhenNoPersons_ReturnsEmptyList()
    {
        // Act
        var result = await _service.GetAllPersonsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllPersonsAsync_WhenPersonsExist_ReturnsAllPersons()
    {
        // Arrange
        var persons = new List<Person>
        {
            new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 },
            new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 },
            new Person { FirstName = "Bob", LastName = "Johnson", Email = "bob@test.com", Age = 35 }
        };

        foreach (var person in persons)
        {
            await _service.CreatePersonAsync(person);
        }

        // Act
        var result = await _service.GetAllPersonsAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.FirstName == "John" && p.LastName == "Doe");
        result.Should().Contain(p => p.FirstName == "Jane" && p.LastName == "Smith");
        result.Should().Contain(p => p.FirstName == "Bob" && p.LastName == "Johnson");
    }

    #endregion

    #region GetPersonByIdAsync Tests

    [Fact]
    public async Task GetPersonByIdAsync_WhenPersonExists_ReturnsPerson()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };
        var createdPerson = await _service.CreatePersonAsync(person);

        // Act
        var result = await _service.GetPersonByIdAsync(createdPerson.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdPerson.Id);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john@test.com");
        result.Age.Should().Be(30);
    }

    [Fact]
    public async Task GetPersonByIdAsync_WhenPersonDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _service.GetPersonByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPersonByIdAsync_WithNegativeId_ReturnsNull()
    {
        // Act
        var result = await _service.GetPersonByIdAsync(-1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPersonByIdAsync_WithZeroId_ReturnsNull()
    {
        // Act
        var result = await _service.GetPersonByIdAsync(0);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreatePersonAsync Tests

    [Fact]
    public async Task CreatePersonAsync_WithValidPerson_CreatesPersonWithId()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };

        // Act
        var result = await _service.CreatePersonAsync(person);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john@test.com");
        result.Age.Should().Be(30);
    }

    [Fact]
    public async Task CreatePersonAsync_MultiplePersons_AssignsIncrementalIds()
    {
        // Arrange
        var person1 = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var person2 = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };
        var person3 = new Person { FirstName = "Bob", LastName = "Johnson", Email = "bob@test.com", Age = 35 };

        // Act
        var result1 = await _service.CreatePersonAsync(person1);
        var result2 = await _service.CreatePersonAsync(person2);
        var result3 = await _service.CreatePersonAsync(person3);

        // Assert
        result1.Id.Should().Be(1);
        result2.Id.Should().Be(2);
        result3.Id.Should().Be(3);
    }

    [Fact]
    public async Task CreatePersonAsync_WithEmptyStrings_CreatesPersonSuccessfully()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "",
            LastName = "",
            Email = "",
            Age = 0
        };

        // Act
        var result = await _service.CreatePersonAsync(person);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.FirstName.Should().Be("");
        result.LastName.Should().Be("");
        result.Email.Should().Be("");
        result.Age.Should().Be(0);
    }

    [Fact]
    public async Task CreatePersonAsync_WithMaxAge_CreatesPersonSuccessfully()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "Old",
            LastName = "Person",
            Email = "old@test.com",
            Age = int.MaxValue
        };

        // Act
        var result = await _service.CreatePersonAsync(person);

        // Assert
        result.Should().NotBeNull();
        result.Age.Should().Be(int.MaxValue);
    }

    #endregion

    #region UpdatePersonAsync Tests

    [Fact]
    public async Task UpdatePersonAsync_WhenPersonExists_UpdatesPersonSuccessfully()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };
        var createdPerson = await _service.CreatePersonAsync(person);

        var updatedPerson = new Person
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            Age = 25
        };

        // Act
        var result = await _service.UpdatePersonAsync(createdPerson.Id, updatedPerson);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdPerson.Id);
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        result.Email.Should().Be("jane@test.com");
        result.Age.Should().Be(25);
    }

    [Fact]
    public async Task UpdatePersonAsync_WhenPersonDoesNotExist_ReturnsNull()
    {
        // Arrange
        var updatedPerson = new Person
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            Age = 25
        };

        // Act
        var result = await _service.UpdatePersonAsync(999, updatedPerson);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdatePersonAsync_WithPartialUpdate_UpdatesOnlyProvidedFields()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };
        var createdPerson = await _service.CreatePersonAsync(person);

        var updatedPerson = new Person
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };

        // Act
        var result = await _service.UpdatePersonAsync(createdPerson.Id, updatedPerson);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john@test.com");
        result.Age.Should().Be(30);
    }

    [Fact]
    public async Task UpdatePersonAsync_PersistsChanges()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };
        var createdPerson = await _service.CreatePersonAsync(person);

        var updatedPerson = new Person
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            Age = 25
        };

        // Act
        await _service.UpdatePersonAsync(createdPerson.Id, updatedPerson);
        var result = await _service.GetPersonByIdAsync(createdPerson.Id);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
    }

    #endregion

    #region DeletePersonAsync Tests

    [Fact]
    public async Task DeletePersonAsync_WhenPersonExists_DeletesPersonAndReturnsTrue()
    {
        // Arrange
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };
        var createdPerson = await _service.CreatePersonAsync(person);

        // Act
        var result = await _service.DeletePersonAsync(createdPerson.Id);

        // Assert
        result.Should().BeTrue();
        var deletedPerson = await _service.GetPersonByIdAsync(createdPerson.Id);
        deletedPerson.Should().BeNull();
    }

    [Fact]
    public async Task DeletePersonAsync_WhenPersonDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _service.DeletePersonAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeletePersonAsync_WithNegativeId_ReturnsFalse()
    {
        // Act
        var result = await _service.DeletePersonAsync(-1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeletePersonAsync_WithZeroId_ReturnsFalse()
    {
        // Act
        var result = await _service.DeletePersonAsync(0);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeletePersonAsync_RemovesPersonFromDatabase()
    {
        // Arrange
        var person1 = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var person2 = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };
        var createdPerson1 = await _service.CreatePersonAsync(person1);
        await _service.CreatePersonAsync(person2);

        // Act
        await _service.DeletePersonAsync(createdPerson1.Id);
        var allPersons = await _service.GetAllPersonsAsync();

        // Assert
        allPersons.Should().HaveCount(1);
        allPersons.Should().NotContain(p => p.Id == createdPerson1.Id);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task CompleteWorkflow_CreateUpdateDelete_WorksCorrectly()
    {
        // Create
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Age = 30
        };
        var created = await _service.CreatePersonAsync(person);
        created.Id.Should().BeGreaterThan(0);

        // Read
        var retrieved = await _service.GetPersonByIdAsync(created.Id);
        retrieved.Should().NotBeNull();
        retrieved!.FirstName.Should().Be("John");

        // Update
        var updated = new Person
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            Age = 25
        };
        var updateResult = await _service.UpdatePersonAsync(created.Id, updated);
        updateResult.Should().NotBeNull();
        updateResult!.FirstName.Should().Be("Jane");

        // Delete
        var deleteResult = await _service.DeletePersonAsync(created.Id);
        deleteResult.Should().BeTrue();

        // Verify deletion
        var deletedPerson = await _service.GetPersonByIdAsync(created.Id);
        deletedPerson.Should().BeNull();
    }

    #endregion

    #region GetPersonCitiesAsync Tests

    [Fact]
    public async Task GetPersonCitiesAsync_ReturnsEmptyList_WhenPersonHasNoCities()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);

        // Act
        var result = await _service.GetPersonCitiesAsync(createdPerson.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPersonCitiesAsync_ReturnsCitiesWithDetails_WhenPersonHasCities()
    {
        // Arrange
        var city1 = new City { Id = 1, Name = "Paris", Country = "France" };
        var city2 = new City { Id = 2, Name = "Tokyo", Country = "Japan" };
        _context.Cities.AddRange(city1, city2);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);

        await _service.AddCityToPersonAsync(createdPerson.Id, 1);
        await _service.AddCityToPersonAsync(createdPerson.Id, 2);

        // Act
        var result = await _service.GetPersonCitiesAsync(createdPerson.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(pc => pc.City.Name == "Paris" && pc.City.Country == "France");
        result.Should().Contain(pc => pc.City.Name == "Tokyo" && pc.City.Country == "Japan");
    }

    [Fact]
    public async Task GetPersonCitiesAsync_ReturnsOnlyPersonsCities_NotOtherPersonsCities()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person1 = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var person2 = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };
        var createdPerson1 = await _service.CreatePersonAsync(person1);
        var createdPerson2 = await _service.CreatePersonAsync(person2);

        await _service.AddCityToPersonAsync(createdPerson1.Id, 1);

        // Act
        var result = await _service.GetPersonCitiesAsync(createdPerson2.Id);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region AddCityToPersonAsync Tests

    [Fact]
    public async Task AddCityToPersonAsync_AddsCity_WhenBothPersonAndCityExist()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);

        // Act
        var result = await _service.AddCityToPersonAsync(createdPerson.Id, 1);

        // Assert
        result.Should().NotBeNull();
        result!.PersonId.Should().Be(createdPerson.Id);
        result.CityId.Should().Be(1);
        result.IsVisited.Should().BeFalse();
        result.VisitedDate.Should().BeNull();
        result.City.Should().NotBeNull();
        result.City.Name.Should().Be("Paris");
    }

    [Fact]
    public async Task AddCityToPersonAsync_ReturnsNull_WhenPersonDoesNotExist()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.AddCityToPersonAsync(999, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddCityToPersonAsync_ReturnsNull_WhenCityDoesNotExist()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);

        // Act
        var result = await _service.AddCityToPersonAsync(createdPerson.Id, 999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddCityToPersonAsync_ReturnsExistingEntry_WhenCityAlreadyAdded()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);

        await _service.AddCityToPersonAsync(createdPerson.Id, 1);

        // Act
        var result = await _service.AddCityToPersonAsync(createdPerson.Id, 1);

        // Assert
        result.Should().NotBeNull();
        var cities = await _service.GetPersonCitiesAsync(createdPerson.Id);
        cities.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddCityToPersonAsync_AllowsMultipleCities_ForSamePerson()
    {
        // Arrange
        var city1 = new City { Id = 1, Name = "Paris", Country = "France" };
        var city2 = new City { Id = 2, Name = "Tokyo", Country = "Japan" };
        var city3 = new City { Id = 3, Name = "New York", Country = "USA" };
        _context.Cities.AddRange(city1, city2, city3);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);

        // Act
        await _service.AddCityToPersonAsync(createdPerson.Id, 1);
        await _service.AddCityToPersonAsync(createdPerson.Id, 2);
        await _service.AddCityToPersonAsync(createdPerson.Id, 3);

        // Assert
        var cities = await _service.GetPersonCitiesAsync(createdPerson.Id);
        cities.Should().HaveCount(3);
    }

    #endregion

    #region MarkCityAsVisitedAsync Tests

    [Fact]
    public async Task MarkCityAsVisitedAsync_MarksAsVisited_WhenRelationshipExists()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);
        await _service.AddCityToPersonAsync(createdPerson.Id, 1);

        // Act
        var result = await _service.MarkCityAsVisitedAsync(createdPerson.Id, 1, true);

        // Assert
        result.Should().NotBeNull();
        result!.IsVisited.Should().BeTrue();
        result.VisitedDate.Should().NotBeNull();
        result.VisitedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task MarkCityAsVisitedAsync_UnmarksAsVisited_WhenSettingToFalse()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);
        await _service.AddCityToPersonAsync(createdPerson.Id, 1);
        await _service.MarkCityAsVisitedAsync(createdPerson.Id, 1, true);

        // Act
        var result = await _service.MarkCityAsVisitedAsync(createdPerson.Id, 1, false);

        // Assert
        result.Should().NotBeNull();
        result!.IsVisited.Should().BeFalse();
        result.VisitedDate.Should().BeNull();
    }

    [Fact]
    public async Task MarkCityAsVisitedAsync_AcceptsCustomVisitedDate()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);
        await _service.AddCityToPersonAsync(createdPerson.Id, 1);

        var customDate = new DateTime(2023, 6, 15);

        // Act
        var result = await _service.MarkCityAsVisitedAsync(createdPerson.Id, 1, true, customDate);

        // Assert
        result.Should().NotBeNull();
        result!.IsVisited.Should().BeTrue();
        result.VisitedDate.Should().Be(customDate);
    }

    [Fact]
    public async Task MarkCityAsVisitedAsync_ReturnsNull_WhenRelationshipDoesNotExist()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);

        // Act
        var result = await _service.MarkCityAsVisitedAsync(createdPerson.Id, 1, true);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task MarkCityAsVisitedAsync_ReturnsNull_WhenPersonDoesNotExist()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.MarkCityAsVisitedAsync(999, 1, true);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task MarkCityAsVisitedAsync_PersistsChanges()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);
        await _service.AddCityToPersonAsync(createdPerson.Id, 1);

        // Act
        await _service.MarkCityAsVisitedAsync(createdPerson.Id, 1, true);
        var cities = await _service.GetPersonCitiesAsync(createdPerson.Id);

        // Assert
        cities.First().IsVisited.Should().BeTrue();
        cities.First().VisitedDate.Should().NotBeNull();
    }

    #endregion

    #region RemoveCityFromPersonAsync Tests

    [Fact]
    public async Task RemoveCityFromPersonAsync_RemovesCity_WhenRelationshipExists()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);
        await _service.AddCityToPersonAsync(createdPerson.Id, 1);

        // Act
        var result = await _service.RemoveCityFromPersonAsync(createdPerson.Id, 1);

        // Assert
        result.Should().BeTrue();
        var cities = await _service.GetPersonCitiesAsync(createdPerson.Id);
        cities.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveCityFromPersonAsync_ReturnsFalse_WhenRelationshipDoesNotExist()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);

        // Act
        var result = await _service.RemoveCityFromPersonAsync(createdPerson.Id, 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveCityFromPersonAsync_ReturnsFalse_WhenPersonDoesNotExist()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.RemoveCityFromPersonAsync(999, 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveCityFromPersonAsync_OnlyRemovesSpecificCity()
    {
        // Arrange
        var city1 = new City { Id = 1, Name = "Paris", Country = "France" };
        var city2 = new City { Id = 2, Name = "Tokyo", Country = "Japan" };
        _context.Cities.AddRange(city1, city2);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);
        await _service.AddCityToPersonAsync(createdPerson.Id, 1);
        await _service.AddCityToPersonAsync(createdPerson.Id, 2);

        // Act
        await _service.RemoveCityFromPersonAsync(createdPerson.Id, 1);

        // Assert
        var cities = await _service.GetPersonCitiesAsync(createdPerson.Id);
        cities.Should().HaveCount(1);
        cities.Should().Contain(pc => pc.CityId == 2);
        cities.Should().NotContain(pc => pc.CityId == 1);
    }

    [Fact]
    public async Task RemoveCityFromPersonAsync_DoesNotAffectOtherPersons()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person1 = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var person2 = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };
        var createdPerson1 = await _service.CreatePersonAsync(person1);
        var createdPerson2 = await _service.CreatePersonAsync(person2);

        await _service.AddCityToPersonAsync(createdPerson1.Id, 1);
        await _service.AddCityToPersonAsync(createdPerson2.Id, 1);

        // Act
        await _service.RemoveCityFromPersonAsync(createdPerson1.Id, 1);

        // Assert
        var person1Cities = await _service.GetPersonCitiesAsync(createdPerson1.Id);
        var person2Cities = await _service.GetPersonCitiesAsync(createdPerson2.Id);

        person1Cities.Should().BeEmpty();
        person2Cities.Should().HaveCount(1);
    }

    #endregion

    #region City Integration Tests

    [Fact]
    public async Task CityWorkflow_AddMarkAsVisitedRemove_WorksCorrectly()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Paris", Country = "France" };
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);

        // Add city
        var addResult = await _service.AddCityToPersonAsync(createdPerson.Id, 1);
        addResult.Should().NotBeNull();
        addResult!.IsVisited.Should().BeFalse();

        // Mark as visited
        var markResult = await _service.MarkCityAsVisitedAsync(createdPerson.Id, 1, true);
        markResult.Should().NotBeNull();
        markResult!.IsVisited.Should().BeTrue();

        // Remove city
        var removeResult = await _service.RemoveCityFromPersonAsync(createdPerson.Id, 1);
        removeResult.Should().BeTrue();

        // Verify removal
        var cities = await _service.GetPersonCitiesAsync(createdPerson.Id);
        cities.Should().BeEmpty();
    }

    [Fact]
    public async Task PersonWithMultipleCities_CanManageEachIndependently()
    {
        // Arrange
        var city1 = new City { Id = 1, Name = "Paris", Country = "France" };
        var city2 = new City { Id = 2, Name = "Tokyo", Country = "Japan" };
        var city3 = new City { Id = 3, Name = "New York", Country = "USA" };
        _context.Cities.AddRange(city1, city2, city3);
        await _context.SaveChangesAsync();

        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createdPerson = await _service.CreatePersonAsync(person);

        // Add three cities
        await _service.AddCityToPersonAsync(createdPerson.Id, 1);
        await _service.AddCityToPersonAsync(createdPerson.Id, 2);
        await _service.AddCityToPersonAsync(createdPerson.Id, 3);

        // Mark only Paris as visited
        await _service.MarkCityAsVisitedAsync(createdPerson.Id, 1, true);

        // Act
        var cities = await _service.GetPersonCitiesAsync(createdPerson.Id);

        // Assert
        cities.Should().HaveCount(3);
        cities.Single(pc => pc.CityId == 1).IsVisited.Should().BeTrue();
        cities.Single(pc => pc.CityId == 2).IsVisited.Should().BeFalse();
        cities.Single(pc => pc.CityId == 3).IsVisited.Should().BeFalse();
    }

    #endregion
}
