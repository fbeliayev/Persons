import type { City, PersonCity } from '../types/City';

const API_URL = 'http://localhost:5126/api';

export const cityService = {
  async getAllCities(): Promise<City[]> {
    const response = await fetch(`${API_URL}/cities`);
    if (!response.ok) {
      throw new Error('Failed to fetch cities');
    }
    return response.json();
  },

  async getPersonCities(personId: number): Promise<PersonCity[]> {
    const response = await fetch(`${API_URL}/persons/${personId}/cities`);
    if (!response.ok) {
      throw new Error('Failed to fetch person cities');
    }
    return response.json();
  },

  async addCityToPerson(personId: number, cityId: number): Promise<PersonCity> {
    const response = await fetch(`${API_URL}/persons/${personId}/cities/${cityId}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
    });
    if (!response.ok) {
      throw new Error('Failed to add city to person');
    }
    return response.json();
  },

  async updatePersonCity(
    personId: number,
    cityId: number,
    isVisited: boolean,
    visitedDate?: string
  ): Promise<PersonCity> {
    const response = await fetch(`${API_URL}/persons/${personId}/cities/${cityId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ isVisited, visitedDate }),
    });
    if (!response.ok) {
      throw new Error('Failed to update person city');
    }
    return response.json();
  },

  async removeCityFromPerson(personId: number, cityId: number): Promise<void> {
    const response = await fetch(`${API_URL}/persons/${personId}/cities/${cityId}`, {
      method: 'DELETE',
    });
    if (!response.ok) {
      if (response.status === 400) {
        const errorData = await response.json();
        throw new Error(errorData.error || 'Failed to remove city from person');
      }
      throw new Error('Failed to remove city from person');
    }
  },
};
