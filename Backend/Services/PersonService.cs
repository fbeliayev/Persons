using Microsoft.EntityFrameworkCore;
using PersonApi.Data;
using PersonApi.Models;

namespace PersonApi.Services;

public class PersonService : IPersonService
{
    private readonly PersonDbContext _context;

    public PersonService(PersonDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Person>> GetAllPersonsAsync()
    {
        return await _context.Persons.ToListAsync();
    }

    public async Task<Person?> GetPersonByIdAsync(int id)
    {
        return await _context.Persons.FindAsync(id);
    }

    public async Task<Person> CreatePersonAsync(Person person)
    {
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();
        return person;
    }

    public async Task<Person?> UpdatePersonAsync(int id, Person person)
    {
        var existingPerson = await _context.Persons.FindAsync(id);
        if (existingPerson == null)
        {
            return null;
        }

        existingPerson.FirstName = person.FirstName;
        existingPerson.LastName = person.LastName;
        existingPerson.Email = person.Email;
        existingPerson.Age = person.Age;

        await _context.SaveChangesAsync();
        return existingPerson;
    }

    public async Task<bool> DeletePersonAsync(int id)
    {
        var person = await _context.Persons.FindAsync(id);
        if (person == null)
        {
            return false;
        }

        _context.Persons.Remove(person);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PersonCity>> GetPersonCitiesAsync(int personId)
    {
        return await _context.PersonCities
            .Include(pc => pc.City)
            .Where(pc => pc.PersonId == personId)
            .ToListAsync();
    }

    public async Task<PersonCity?> AddCityToPersonAsync(int personId, int cityId)
    {
        var person = await _context.Persons.FindAsync(personId);
        if (person == null)
        {
            return null;
        }

        var city = await _context.Cities.FindAsync(cityId);
        if (city == null)
        {
            return null;
        }

        var existingPersonCity = await _context.PersonCities
            .FirstOrDefaultAsync(pc => pc.PersonId == personId && pc.CityId == cityId);

        if (existingPersonCity != null)
        {
            return existingPersonCity;
        }

        var personCity = new PersonCity
        {
            PersonId = personId,
            CityId = cityId,
            IsVisited = false
        };

        _context.PersonCities.Add(personCity);
        await _context.SaveChangesAsync();

        return await _context.PersonCities
            .Include(pc => pc.City)
            .FirstAsync(pc => pc.PersonId == personId && pc.CityId == cityId);
    }

    public async Task<PersonCity?> MarkCityAsVisitedAsync(int personId, int cityId, bool isVisited, DateTime? visitedDate = null)
    {
        var personCity = await _context.PersonCities
            .Include(pc => pc.City)
            .FirstOrDefaultAsync(pc => pc.PersonId == personId && pc.CityId == cityId);

        if (personCity == null)
        {
            return null;
        }

        personCity.IsVisited = isVisited;
        personCity.VisitedDate = isVisited ? (visitedDate ?? DateTime.UtcNow) : null;

        await _context.SaveChangesAsync();
        return personCity;
    }

    public async Task<bool> RemoveCityFromPersonAsync(int personId, int cityId)
    {
        var personCity = await _context.PersonCities
            .FirstOrDefaultAsync(pc => pc.PersonId == personId && pc.CityId == cityId);

        if (personCity == null)
        {
            return false;
        }

        if (personCity.IsVisited)
        {
            throw new InvalidOperationException("Cannot remove a city that has been visited. Please unmark it as visited first.");
        }

        _context.PersonCities.Remove(personCity);
        await _context.SaveChangesAsync();
        return true;
    }
}
