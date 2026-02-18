using Microsoft.EntityFrameworkCore;
using PersonApi.Data;
using PersonApi.Models;

namespace PersonApi.Services;

public class CityService : ICityService
{
    private readonly PersonDbContext _context;
    private readonly ILogger<CityService> _logger;

    public CityService(PersonDbContext context, ILogger<CityService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<City>> GetAllCitiesAsync()
    {
        _logger.LogInformation("Getting all cities");
        return await _context.Cities.ToListAsync();
    }

    public async Task<City?> GetCityByIdAsync(int id)
    {
        _logger.LogInformation($"Getting city with id {id}");
        return await _context.Cities.FindAsync(id);
    }
}
