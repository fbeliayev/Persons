using Microsoft.AspNetCore.Mvc;
using PersonApi.Models;
using PersonApi.Services;

namespace PersonApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly ILogger<PersonsController> _logger;

    public PersonsController(IPersonService personService, ILogger<PersonsController> logger)
    {
        _personService = personService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Person>>> GetAllPersons()
    {
        try
        {
            var persons = await _personService.GetAllPersonsAsync();
            _logger.LogInformation($"Retrieved {persons.Count()} persons");
            return Ok(persons);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving persons");
            return StatusCode(500, "Error retrieving persons");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Person>> GetPerson(int id)
    {
        try
        {
            var person = await _personService.GetPersonByIdAsync(id);
            if (person == null)
            {
                _logger.LogWarning($"Person with id {id} not found");
                return NotFound();
            }
            return Ok(person);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving person with id {id}");
            return StatusCode(500, "Error retrieving person");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Person>> CreatePerson(Person person)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid person data received: {ValidationErrors}", 
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Creating person {FirstName} {LastName} with email {Email}",
                person.FirstName, person.LastName, person.Email);

            var createdPerson = await _personService.CreatePersonAsync(person);

            _logger.LogInformation("Person created successfully with ID {PersonId}", createdPerson.Id);

            return CreatedAtAction(nameof(GetPerson), new { id = createdPerson.Id }, createdPerson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating person {FirstName} {LastName}", 
                person.FirstName, person.LastName);
            return StatusCode(500, $"Error creating person: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Person>> UpdatePerson(int id, Person person)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid person data received for update: {ValidationErrors}", 
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Updating person {PersonId}", id);
            var updatedPerson = await _personService.UpdatePersonAsync(id, person);
            if (updatedPerson == null)
            {
                _logger.LogWarning("Person {PersonId} not found for update", id);
                return NotFound();
            }
            _logger.LogInformation("Person {PersonId} updated successfully", id);
            return Ok(updatedPerson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating person {PersonId}", id);
            return StatusCode(500, "Error updating person");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson(int id)
    {
        try
        {
            _logger.LogInformation($"Deleting person with id {id}");
            var result = await _personService.DeletePersonAsync(id);
            if (!result)
            {
                _logger.LogWarning($"Person with id {id} not found for deletion");
                return NotFound();
            }
            _logger.LogInformation($"Person with id {id} deleted successfully");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting person with id {id}");
            return StatusCode(500, "Error deleting person");
        }
    }

    [HttpGet("{id}/cities")]
    public async Task<ActionResult<IEnumerable<PersonCity>>> GetPersonCities(int id)
    {
        try
        {
            var person = await _personService.GetPersonByIdAsync(id);
            if (person == null)
            {
                _logger.LogWarning($"Person with id {id} not found");
                return NotFound();
            }

            var cities = await _personService.GetPersonCitiesAsync(id);
            _logger.LogInformation($"Retrieved {cities.Count()} cities for person {id}");
            return Ok(cities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving cities for person {id}");
            return StatusCode(500, "Error retrieving person cities");
        }
    }

    [HttpPost("{id}/cities/{cityId}")]
    public async Task<ActionResult<PersonCity>> AddCityToPerson(int id, int cityId)
    {
        try
        {
            _logger.LogInformation($"Adding city {cityId} to person {id}");
            var personCity = await _personService.AddCityToPersonAsync(id, cityId);
            if (personCity == null)
            {
                _logger.LogWarning($"Person {id} or city {cityId} not found");
                return NotFound("Person or city not found");
            }
            _logger.LogInformation($"City {cityId} added to person {id}");
            return Ok(personCity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding city {cityId} to person {id}");
            return StatusCode(500, "Error adding city to person");
        }
    }

    [HttpPut("{id}/cities/{cityId}")]
    public async Task<ActionResult<PersonCity>> UpdatePersonCity(int id, int cityId, [FromBody] UpdatePersonCityRequest request)
    {
        try
        {
            _logger.LogInformation($"Updating city {cityId} for person {id}");
            var personCity = await _personService.MarkCityAsVisitedAsync(id, cityId, request.IsVisited, request.VisitedDate);
            if (personCity == null)
            {
                _logger.LogWarning($"Person {id} or city {cityId} relationship not found");
                return NotFound();
            }
            _logger.LogInformation($"City {cityId} updated for person {id}");
            return Ok(personCity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating city {cityId} for person {id}");
            return StatusCode(500, "Error updating person city");
        }
    }

    [HttpDelete("{id}/cities/{cityId}")]
    public async Task<IActionResult> RemoveCityFromPerson(int id, int cityId)
    {
        try
        {
            _logger.LogInformation($"Removing city {cityId} from person {id}");
            var result = await _personService.RemoveCityFromPersonAsync(id, cityId);
            if (!result)
            {
                _logger.LogWarning($"Person {id} or city {cityId} relationship not found");
                return NotFound();
            }
            _logger.LogInformation($"City {cityId} removed from person {id}");
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, $"Cannot remove visited city {cityId} from person {id}");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing city {cityId} from person {id}");
            return StatusCode(500, "Error removing city from person");
        }
    }
}

public class UpdatePersonCityRequest
{
    public bool IsVisited { get; set; }
    public DateTime? VisitedDate { get; set; }
}
