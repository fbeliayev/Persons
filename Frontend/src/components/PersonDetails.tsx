import { useState, useEffect } from 'react';
import type { Person } from '../types/Person';
import type { City, PersonCity } from '../types/City';
import { cityService } from '../services/cityService';
import './PersonDetails.css';

interface PersonDetailsProps {
  person: Person;
  onClose: () => void;
}

const PersonDetails = ({ person, onClose }: PersonDetailsProps) => {
  const [allCities, setAllCities] = useState<City[]>([]);
  const [personCities, setPersonCities] = useState<PersonCity[]>([]);
  const [selectedCityIds, setSelectedCityIds] = useState<number[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadData();
  }, [person.id]);

  const loadData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [cities, pCities] = await Promise.all([
        cityService.getAllCities(),
        cityService.getPersonCities(person.id),
      ]);
      setAllCities(cities);
      setPersonCities(pCities);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load data');
    } finally {
      setLoading(false);
    }
  };

  const handleAddCities = async () => {
    if (selectedCityIds.length === 0) return;

    setLoading(true);
    setError(null);
    try {
      await Promise.all(
        selectedCityIds.map((cityId) =>
          cityService.addCityToPerson(person.id, cityId)
        )
      );
      await loadData();
      setSelectedCityIds([]);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to add cities');
    } finally {
      setLoading(false);
    }
  };

  const handleToggleVisited = async (cityId: number, isVisited: boolean) => {
    setLoading(true);
    setError(null);
    try {
      await cityService.updatePersonCity(
        person.id,
        cityId,
        isVisited,
        isVisited ? new Date().toISOString() : undefined
      );
      await loadData();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update city');
    } finally {
      setLoading(false);
    }
  };

  const handleRemoveCity = async (cityId: number, isVisited: boolean) => {
    if (isVisited) {
      alert('Cannot remove a visited city. Please unmark it as visited first.');
      return;
    }

    if (!window.confirm('Remove this city from your list?')) return;

    setLoading(true);
    setError(null);
    try {
      await cityService.removeCityFromPerson(person.id, cityId);
      await loadData();
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to remove city';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const availableCities = allCities.filter(
    (city) => !personCities.some((pc) => pc.cityId === city.id)
  );

  const handleSelectChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const options = e.target.options;
    const selected: number[] = [];
    for (let i = 0; i < options.length; i++) {
      if (options[i].selected) {
        selected.push(parseInt(options[i].value));
      }
    }
    setSelectedCityIds(selected);
  };

  return (
    <div className="person-details-overlay" onClick={onClose}>
      <div className="person-details-modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>
            Cities for {person.firstName} {person.lastName}
          </h2>
          <button className="btn-close" onClick={onClose}>
            ×
          </button>
        </div>

        {error && <div className="error-message">{error}</div>}

        <div className="modal-content">
          <div className="add-cities-section">
            <h3>Add Cities to Visit</h3>
            <div className="add-cities-form">
              <select
                multiple
                value={selectedCityIds.map(String)}
                onChange={handleSelectChange}
                className="city-multiselect"
                disabled={loading || availableCities.length === 0}
              >
                {availableCities.map((city) => (
                  <option key={city.id} value={city.id}>
                    {city.name}, {city.country}
                  </option>
                ))}
              </select>
              <button
                className="btn btn-primary"
                onClick={handleAddCities}
                disabled={loading || selectedCityIds.length === 0}
              >
                Add Selected Cities
              </button>
            </div>
            {availableCities.length === 0 && (
              <p className="info-text">All cities have been added!</p>
            )}
            <p className="help-text">Hold Ctrl (Cmd on Mac) to select multiple cities</p>
          </div>

          <div className="person-cities-section">
            <h3>My Cities ({personCities.length})</h3>
            {personCities.length === 0 ? (
              <p className="empty-text">No cities added yet. Add some cities to visit!</p>
            ) : (
              <div className="cities-grid">
                {personCities.map((pc) => (
                  <div
                    key={pc.cityId}
                    className={`city-card ${pc.isVisited ? 'visited' : ''}`}
                  >
                    <div className="city-info">
                      <h4>{pc.city.name}</h4>
                      <p className="city-country">{pc.city.country}</p>
                      {pc.isVisited && pc.visitedDate && (
                        <p className="visited-date">
                          Visited: {new Date(pc.visitedDate).toLocaleDateString()}
                        </p>
                      )}
                    </div>
                    <div className="city-actions">
                      <button
                        className={`btn ${pc.isVisited ? 'btn-secondary' : 'btn-success'}`}
                        onClick={() => handleToggleVisited(pc.cityId, !pc.isVisited)}
                        disabled={loading}
                      >
                        {pc.isVisited ? '✓ Visited' : 'Mark as Visited'}
                      </button>
                      <button
                        className="btn btn-danger"
                        onClick={() => handleRemoveCity(pc.cityId, pc.isVisited)}
                        disabled={loading || pc.isVisited}
                        title={pc.isVisited ? 'Cannot remove visited cities' : 'Remove city'}
                      >
                        Remove
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        <div className="modal-footer">
          <button className="btn btn-secondary" onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
};

export default PersonDetails;
