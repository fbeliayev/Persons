using PersonApi.Models;

namespace PersonApi.Services;

public interface IPersonService
{
    Task<IEnumerable<Person>> GetAllPersonsAsync();
    Task<Person?> GetPersonByIdAsync(int id);
    Task<Person> CreatePersonAsync(Person person);
    Task<Person?> UpdatePersonAsync(int id, Person person);
    Task<bool> DeletePersonAsync(int id);

    Task<IEnumerable<PersonCity>> GetPersonCitiesAsync(int personId);
    Task<PersonCity?> AddCityToPersonAsync(int personId, int cityId);
    Task<PersonCity?> MarkCityAsVisitedAsync(int personId, int cityId, bool isVisited, DateTime? visitedDate = null);
    Task<bool> RemoveCityFromPersonAsync(int personId, int cityId);
}
