using PersonApi.Models;

namespace PersonApi.Services;

public interface ICityService
{
    Task<IEnumerable<City>> GetAllCitiesAsync();
    Task<City?> GetCityByIdAsync(int id);
}
