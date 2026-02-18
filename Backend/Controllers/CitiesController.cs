using Microsoft.AspNetCore.Mvc;
using PersonApi.Models;
using PersonApi.Services;

namespace PersonApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;
    private readonly ILogger<CitiesController> _logger;

    public CitiesController(ICityService cityService, ILogger<CitiesController> logger)
    {
        _cityService = cityService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all available cities
    /// </summary>
    /// <returns>List of cities</returns>
    [HttpGet]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(typeof(IEnumerable<City>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<City>>> GetAllCities()
    {
        try
        {
            var cities = await _cityService.GetAllCitiesAsync();
            _logger.LogInformation("Retrieved {CityCount} cities", cities.Count());
            return Ok(cities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cities");
            return StatusCode(500, "Error retrieving cities");
        }
    }

    /// <summary>
    /// Gets a specific city by ID
    /// </summary>
    /// <param name="id">City ID</param>
    /// <returns>The requested city</returns>
    [HttpGet("{id}")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    [ProducesResponseType(typeof(City), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<City>> GetCity(int id)
    {
        try
        {
            var city = await _cityService.GetCityByIdAsync(id);
            if (city == null)
            {
                _logger.LogWarning("City {CityId} not found", id);
                return NotFound();
            }
            return Ok(city);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving city {CityId}", id);
            return StatusCode(500, "Error retrieving city");
        }
    }
}
