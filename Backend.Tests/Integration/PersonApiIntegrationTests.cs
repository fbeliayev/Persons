using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PersonApi.Models;
using Xunit;

namespace PersonApi.Tests.Integration;

public class PersonApiIntegrationTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public PersonApiIntegrationTests()
    {
        _factory = new CustomWebApplicationFactory();
    }

    public Task InitializeAsync()
    {
        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        await _factory.DisposeAsync();
    }

    #region GET /api/persons Tests

    [Fact]
    public async Task GetAllPersons_ReturnsEmptyList_Initially()
    {
        // Act
        var response = await _client.GetAsync("/api/persons");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var persons = await response.Content.ReadFromJsonAsync<List<Person>>();
        persons.Should().NotBeNull();
        persons.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllPersons_ReturnsCreatedPersons()
    {
        // Arrange
        var person1 = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var person2 = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };

        await _client.PostAsJsonAsync("/api/persons", person1);
        await _client.PostAsJsonAsync("/api/persons", person2);

        // Act
        var response = await _client.GetAsync("/api/persons");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var persons = await response.Content.ReadFromJsonAsync<List<Person>>();
        persons.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    #endregion

    #region POST /api/persons Tests

    [Fact]
    public async Task CreatePerson_ReturnsCreatedPerson_WithGeneratedId()
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
        var response = await _client.PostAsJsonAsync("/api/persons", person);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdPerson = await response.Content.ReadFromJsonAsync<Person>();
        createdPerson.Should().NotBeNull();
        createdPerson!.Id.Should().BeGreaterThan(0);
        createdPerson.FirstName.Should().Be("John");
        createdPerson.LastName.Should().Be("Doe");
        createdPerson.Email.Should().Be("john@test.com");
        createdPerson.Age.Should().Be(30);
    }

    [Fact]
    public async Task CreatePerson_ReturnsLocationHeader()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/persons", person);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/Persons/");
    }

    [Fact]
    public async Task CreatePerson_WithEmptyFields_Succeeds()
    {
        // Arrange
        var person = new Person { FirstName = "", LastName = "", Email = "", Age = 0 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/persons", person);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    #endregion

    #region GET /api/persons/{id} Tests

    [Fact]
    public async Task GetPersonById_ReturnsCorrectPerson()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        // Act
        var response = await _client.GetAsync($"/api/persons/{createdPerson!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedPerson = await response.Content.ReadFromJsonAsync<Person>();
        retrievedPerson.Should().NotBeNull();
        retrievedPerson!.Id.Should().Be(createdPerson.Id);
        retrievedPerson.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetPersonById_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/persons/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region PUT /api/persons/{id} Tests

    [Fact]
    public async Task UpdatePerson_UpdatesExistingPerson()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        var updatedPerson = new Person
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            Age = 25
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/persons/{createdPerson!.Id}", updatedPerson);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Person>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdPerson.Id);
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
    }

    [Fact]
    public async Task UpdatePerson_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        var person = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };

        // Act
        var response = await _client.PutAsJsonAsync("/api/persons/99999", person);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePerson_PersistsChanges()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        var updatedPerson = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };
        await _client.PutAsJsonAsync($"/api/persons/{createdPerson!.Id}", updatedPerson);

        // Act
        var getResponse = await _client.GetAsync($"/api/persons/{createdPerson.Id}");

        // Assert
        var retrievedPerson = await getResponse.Content.ReadFromJsonAsync<Person>();
        retrievedPerson!.FirstName.Should().Be("Jane");
        retrievedPerson.LastName.Should().Be("Smith");
    }

    #endregion

    #region DELETE /api/persons/{id} Tests

    [Fact]
    public async Task DeletePerson_DeletesExistingPerson()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        // Act
        var response = await _client.DeleteAsync($"/api/persons/{createdPerson!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePerson_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/persons/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePerson_RemovesPersonPermanently()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        // Act
        await _client.DeleteAsync($"/api/persons/{createdPerson!.Id}");
        var getResponse = await _client.GetAsync($"/api/persons/{createdPerson.Id}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Full Workflow Tests

    [Fact]
    public async Task FullCrudWorkflow_WorksCorrectly()
    {
        // Create
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();
        createdPerson.Should().NotBeNull();

        // Read
        var getResponse = await _client.GetAsync($"/api/persons/{createdPerson!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Update
        var updatedPerson = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };
        var updateResponse = await _client.PutAsJsonAsync($"/api/persons/{createdPerson.Id}", updatedPerson);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Delete
        var deleteResponse = await _client.DeleteAsync($"/api/persons/{createdPerson.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var finalGetResponse = await _client.GetAsync($"/api/persons/{createdPerson.Id}");
        finalGetResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateMultiplePersons_AssignsUniqueIds()
    {
        // Arrange
        var person1 = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var person2 = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };
        var person3 = new Person { FirstName = "Bob", LastName = "Johnson", Email = "bob@test.com", Age = 35 };

        // Act
        var response1 = await _client.PostAsJsonAsync("/api/persons", person1);
        var response2 = await _client.PostAsJsonAsync("/api/persons", person2);
        var response3 = await _client.PostAsJsonAsync("/api/persons", person3);

        // Assert
        var created1 = await response1.Content.ReadFromJsonAsync<Person>();
        var created2 = await response2.Content.ReadFromJsonAsync<Person>();
        var created3 = await response3.Content.ReadFromJsonAsync<Person>();

        created1!.Id.Should().NotBe(created2!.Id);
        created2.Id.Should().NotBe(created3!.Id);
        created1.Id.Should().NotBe(created3.Id);
    }

    #endregion

    #region GET /api/cities Tests

    [Fact]
    public async Task GetAllCities_ReturnsSeededCities()
    {
        // Act
        var response = await _client.GetAsync("/api/cities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cities = await response.Content.ReadFromJsonAsync<List<City>>();
        cities.Should().NotBeNull();
        cities.Should().HaveCount(10);
        cities.Should().Contain(c => c.Name == "Paris" && c.Country == "France");
        cities.Should().Contain(c => c.Name == "Tokyo" && c.Country == "Japan");
        cities.Should().Contain(c => c.Name == "London" && c.Country == "UK");
    }

    [Fact]
    public async Task GetCityById_ReturnsCorrectCity()
    {
        // Act
        var response = await _client.GetAsync("/api/cities/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var city = await response.Content.ReadFromJsonAsync<City>();
        city.Should().NotBeNull();
        city!.Id.Should().Be(1);
        city.Name.Should().NotBeNullOrEmpty();
        city.Country.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetCityById_ReturnsNotFound_WhenCityDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/cities/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Person Cities Workflow Tests

    [Fact]
    public async Task AddCityToPerson_AddsCity_Successfully()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createPersonResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createPersonResponse.Content.ReadFromJsonAsync<Person>();

        // Act
        var response = await _client.PostAsync($"/api/persons/{createdPerson!.Id}/cities/1", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var personCity = await response.Content.ReadFromJsonAsync<PersonCity>();
        personCity.Should().NotBeNull();
        personCity!.PersonId.Should().Be(createdPerson.Id);
        personCity.CityId.Should().Be(1);
        personCity.IsVisited.Should().BeFalse();
        personCity.City.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPersonCities_ReturnsEmptyList_Initially()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        // Act
        var response = await _client.GetAsync($"/api/persons/{createdPerson!.Id}/cities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cities = await response.Content.ReadFromJsonAsync<List<PersonCity>>();
        cities.Should().NotBeNull();
        cities.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPersonCities_ReturnsAddedCities()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        await _client.PostAsync($"/api/persons/{createdPerson!.Id}/cities/1", null);
        await _client.PostAsync($"/api/persons/{createdPerson.Id}/cities/2", null);

        // Act
        var response = await _client.GetAsync($"/api/persons/{createdPerson.Id}/cities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cities = await response.Content.ReadFromJsonAsync<List<PersonCity>>();
        cities.Should().HaveCount(2);
        cities.Should().Contain(pc => pc.CityId == 1);
        cities.Should().Contain(pc => pc.CityId == 2);
    }

    [Fact]
    public async Task MarkCityAsVisited_UpdatesVisitedStatus()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        await _client.PostAsync($"/api/persons/{createdPerson!.Id}/cities/1", null);

        var updateRequest = new { IsVisited = true, VisitedDate = DateTime.UtcNow };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/persons/{createdPerson.Id}/cities/1", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var personCity = await response.Content.ReadFromJsonAsync<PersonCity>();
        personCity.Should().NotBeNull();
        personCity!.IsVisited.Should().BeTrue();
        personCity.VisitedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task UnmarkCityAsVisited_UpdatesVisitedStatus()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        await _client.PostAsync($"/api/persons/{createdPerson!.Id}/cities/1", null);
        await _client.PutAsJsonAsync($"/api/persons/{createdPerson.Id}/cities/1", new { IsVisited = true, VisitedDate = DateTime.UtcNow });

        // Act
        var response = await _client.PutAsJsonAsync($"/api/persons/{createdPerson.Id}/cities/1", new { IsVisited = false });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var personCity = await response.Content.ReadFromJsonAsync<PersonCity>();
        personCity.Should().NotBeNull();
        personCity!.IsVisited.Should().BeFalse();
        personCity.VisitedDate.Should().BeNull();
    }

    [Fact]
    public async Task RemoveCityFromPerson_RemovesCity()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        await _client.PostAsync($"/api/persons/{createdPerson!.Id}/cities/1", null);

        // Act
        var response = await _client.DeleteAsync($"/api/persons/{createdPerson.Id}/cities/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var citiesResponse = await _client.GetAsync($"/api/persons/{createdPerson.Id}/cities");
        var cities = await citiesResponse.Content.ReadFromJsonAsync<List<PersonCity>>();
        cities.Should().BeEmpty();
    }

    [Fact]
    public async Task AddCityToPerson_ReturnsNotFound_WhenPersonDoesNotExist()
    {
        // Act
        var response = await _client.PostAsync("/api/persons/99999/cities/1", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddCityToPerson_ReturnsNotFound_WhenCityDoesNotExist()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        // Act
        var response = await _client.PostAsync($"/api/persons/{createdPerson!.Id}/cities/999", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddSameCity_Twice_DoesNotDuplicate()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        // Act
        await _client.PostAsync($"/api/persons/{createdPerson!.Id}/cities/1", null);
        await _client.PostAsync($"/api/persons/{createdPerson.Id}/cities/1", null);

        var citiesResponse = await _client.GetAsync($"/api/persons/{createdPerson.Id}/cities");
        var cities = await citiesResponse.Content.ReadFromJsonAsync<List<PersonCity>>();

        // Assert
        cities.Should().HaveCount(1);
    }

    [Fact]
    public async Task CompleteCitiesWorkflow_WorksCorrectly()
    {
        // Arrange - Create person
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        // Add multiple cities
        await _client.PostAsync($"/api/persons/{createdPerson!.Id}/cities/1", null);
        await _client.PostAsync($"/api/persons/{createdPerson.Id}/cities/2", null);
        await _client.PostAsync($"/api/persons/{createdPerson.Id}/cities/3", null);

        // Verify cities added
        var citiesResponse1 = await _client.GetAsync($"/api/persons/{createdPerson.Id}/cities");
        var cities1 = await citiesResponse1.Content.ReadFromJsonAsync<List<PersonCity>>();
        cities1.Should().HaveCount(3);

        // Mark one as visited
        await _client.PutAsJsonAsync($"/api/persons/{createdPerson.Id}/cities/1", new { IsVisited = true, VisitedDate = DateTime.UtcNow });

        // Verify visited status
        var citiesResponse2 = await _client.GetAsync($"/api/persons/{createdPerson.Id}/cities");
        var cities2 = await citiesResponse2.Content.ReadFromJsonAsync<List<PersonCity>>();
        cities2!.Single(c => c.CityId == 1).IsVisited.Should().BeTrue();
        cities2.Single(c => c.CityId == 2).IsVisited.Should().BeFalse();

        // Remove one city
        await _client.DeleteAsync($"/api/persons/{createdPerson.Id}/cities/2");

        // Verify removal
        var citiesResponse3 = await _client.GetAsync($"/api/persons/{createdPerson.Id}/cities");
        var cities3 = await citiesResponse3.Content.ReadFromJsonAsync<List<PersonCity>>();
        cities3.Should().HaveCount(2);
        cities3.Should().NotContain(pc => pc.CityId == 2);
    }

    [Fact]
    public async Task MultiplePeople_CanHaveSameCities_Independently()
    {
        // Arrange
        var person1 = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var person2 = new Person { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Age = 25 };

        var createResponse1 = await _client.PostAsJsonAsync("/api/persons", person1);
        var createResponse2 = await _client.PostAsJsonAsync("/api/persons", person2);

        var createdPerson1 = await createResponse1.Content.ReadFromJsonAsync<Person>();
        var createdPerson2 = await createResponse2.Content.ReadFromJsonAsync<Person>();

        // Add same cities to both persons
        await _client.PostAsync($"/api/persons/{createdPerson1!.Id}/cities/1", null);
        await _client.PostAsync($"/api/persons/{createdPerson2!.Id}/cities/1", null);

        // Mark as visited for person1 only
        await _client.PutAsJsonAsync($"/api/persons/{createdPerson1.Id}/cities/1", new { IsVisited = true, VisitedDate = DateTime.UtcNow });

        // Act
        var cities1Response = await _client.GetAsync($"/api/persons/{createdPerson1.Id}/cities");
        var cities2Response = await _client.GetAsync($"/api/persons/{createdPerson2.Id}/cities");

        var cities1 = await cities1Response.Content.ReadFromJsonAsync<List<PersonCity>>();
        var cities2 = await cities2Response.Content.ReadFromJsonAsync<List<PersonCity>>();

        // Assert
        cities1!.Single(c => c.CityId == 1).IsVisited.Should().BeTrue();
        cities2!.Single(c => c.CityId == 1).IsVisited.Should().BeFalse();
    }

    [Fact]
    public async Task PersonDeletion_DoesNotAffectCities()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        await _client.PostAsync($"/api/persons/{createdPerson!.Id}/cities/1", null);

        // Act - Delete person
        await _client.DeleteAsync($"/api/persons/{createdPerson.Id}");

        // Assert - Cities still exist
        var citiesResponse = await _client.GetAsync("/api/cities");
        var cities = await citiesResponse.Content.ReadFromJsonAsync<List<City>>();
        cities.Should().Contain(c => c.Id == 1);
    }

    [Fact]
    public async Task CustomVisitedDate_CanBeSet()
    {
        // Arrange
        var person = new Person { FirstName = "John", LastName = "Doe", Email = "john@test.com", Age = 30 };
        var createResponse = await _client.PostAsJsonAsync("/api/persons", person);
        var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();

        await _client.PostAsync($"/api/persons/{createdPerson!.Id}/cities/1", null);

        var customDate = new DateTime(2023, 6, 15, 0, 0, 0, DateTimeKind.Utc);
        var updateRequest = new { IsVisited = true, VisitedDate = customDate };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/persons/{createdPerson.Id}/cities/1", updateRequest);

        // Assert
        var personCity = await response.Content.ReadFromJsonAsync<PersonCity>();
        personCity!.VisitedDate.Should().NotBeNull();
        personCity.VisitedDate!.Value.Date.Should().Be(customDate.Date);
    }

    #endregion
}

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class PersonCity
{
    public int PersonId { get; set; }
    public int CityId { get; set; }
    public City City { get; set; } = null!;
    public bool IsVisited { get; set; }
    public DateTime? VisitedDate { get; set; }
}
