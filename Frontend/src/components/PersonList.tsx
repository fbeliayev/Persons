import { useState, useEffect, useCallback } from 'react';
import type { Person } from '../types/Person';
import type { PersonCity } from '../types/City';
import { cityService } from '../services/cityService';
import './PersonList.css';

interface PersonListProps {
  persons: Person[];
  onEdit: (person: Person) => void;
  onDelete: (id: number) => void;
  onViewCities: (person: Person) => void;
  loading: boolean;
}

const PersonList = ({ persons, onEdit, onDelete, onViewCities, loading }: PersonListProps) => {
  const [personCitiesMap, setPersonCitiesMap] = useState<Map<number, PersonCity[]>>(new Map());
  const [loadingCityIds, setLoadingCityIds] = useState<Set<string>>(new Set());
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    const loadCitiesForPersons = async () => {
      try {
        const citiesPromises = persons.map(person => 
          cityService.getPersonCities(person.id)
            .catch(err => {
              console.error(`Failed to load cities for person ${person.id}:`, err);
              return [];
            })
        );

        const citiesResults = await Promise.all(citiesPromises);

        if (isMounted) {
          const newMap = new Map<number, PersonCity[]>();
          persons.forEach((person, index) => {
            newMap.set(person.id, citiesResults[index]);
          });
          setPersonCitiesMap(newMap);
        }
      } catch (error) {
        console.error('Failed to load cities:', error);
      }
    };

    loadCitiesForPersons();

    return () => {
      isMounted = false;
    };
  }, [persons]);

  const handleToggleVisited = useCallback(async (personId: number, cityId: number, cityName: string, isVisited: boolean) => {
    const key = `${personId}-${cityId}`;
    setLoadingCityIds(prev => new Set(prev).add(key));
    setError(null);

    try {
      await cityService.updatePersonCity(
        personId,
        cityId,
        isVisited,
        isVisited ? new Date().toISOString() : undefined
      );

      // Refresh cities for this person
      const cities = await cityService.getPersonCities(personId);
      setPersonCitiesMap(prev => {
        const newMap = new Map(prev);
        newMap.set(personId, cities);
        return newMap;
      });
    } catch (error) {
      const message = error instanceof Error 
        ? `Unable to update ${cityName}: ${error.message}. Please try again.`
        : `An unexpected error occurred while updating ${cityName}. Please refresh and try again.`;
      setError(message);
      console.error('Failed to update city:', error);
    } finally {
      setLoadingCityIds(prev => {
        const newSet = new Set(prev);
        newSet.delete(key);
        return newSet;
      });
    }
  }, []);

  if (loading) {
    return <div className="loading">Loading persons...</div>;
  }

  if (persons.length === 0) {
    return <div className="empty-list">No persons found. Add your first person!</div>;
  }

  return (
    <div className="person-list">
      <h2>Persons List</h2>
      {error && (
        <div className="error-banner">
          {error}
          <button onClick={() => setError(null)} className="error-close">×</button>
        </div>
      )}
      <table>
        <thead>
          <tr>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Email</th>
            <th>Age</th>
            <th>Cities to Visit</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {persons.map((person) => {
            const cities = personCitiesMap.get(person.id) || [];

            return (
              <tr key={person.id}>
                <td>{person.firstName}</td>
                <td>{person.lastName}</td>
                <td>{person.email}</td>
                <td>{person.age}</td>
                <td className="cities-cell">
                  {cities.length === 0 ? (
                    <span className="no-cities">No cities</span>
                  ) : (
                    <div className="cities-list">
                      {cities.map((pc) => {
                        const loadingKey = `${person.id}-${pc.cityId}`;
                        const isLoading = loadingCityIds.has(loadingKey);

                        return (
                          <div key={pc.cityId} className="city-item">
                            <label className="city-checkbox">
                              <input
                                type="checkbox"
                                checked={pc.isVisited}
                                onChange={(e) => handleToggleVisited(person.id, pc.cityId, pc.city.name, e.target.checked)}
                                disabled={isLoading}
                                aria-label={`Mark ${pc.city.name} as visited`}
                              />
                              <span className={pc.isVisited ? 'city-name visited' : 'city-name'}>
                                {pc.city.name}
                                {isLoading && <span className="loading-spinner">⟳</span>}
                              </span>
                            </label>
                          </div>
                        );
                      })}
                    </div>
                  )}
                </td>
                <td className="actions">
                  <button
                    className="btn btn-info"
                    onClick={() => onViewCities(person)}
                    title="View Cities"
                    aria-label={`View cities for ${person.firstName} ${person.lastName}`}
                  >
                    🌍 Cities
                  </button>
                  <button
                    className="btn btn-edit"
                    onClick={() => onEdit(person)}
                    aria-label={`Edit ${person.firstName} ${person.lastName}`}
                  >
                    Edit
                  </button>
                  <button
                    className="btn btn-delete"
                    onClick={() => onDelete(person.id)}
                    aria-label={`Delete ${person.firstName} ${person.lastName}`}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
};

export default PersonList;
