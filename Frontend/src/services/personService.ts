import type { Person } from '../types/Person';

const API_URL = 'http://localhost:5126/api/persons';

export const personService = {
  getAllPersons: async (): Promise<Person[]> => {
    try {
      const response = await fetch(API_URL);
      if (!response.ok) {
        const error = await response.text();
        throw new Error(`Failed to fetch persons: ${response.status} - ${error}`);
      }
      return response.json();
    } catch (error) {
      console.error('Error fetching persons:', error);
      throw error;
    }
  },

  getPersonById: async (id: number): Promise<Person> => {
    try {
      const response = await fetch(`${API_URL}/${id}`);
      if (!response.ok) {
        throw new Error(`Failed to fetch person: ${response.status}`);
      }
      return response.json();
    } catch (error) {
      console.error('Error fetching person:', error);
      throw error;
    }
  },

  createPerson: async (person: Person): Promise<Person> => {
    try {
      console.log('Creating person:', person);
      const response = await fetch(API_URL, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(person),
      });

      if (!response.ok) {
        const error = await response.text();
        console.error('Create person failed:', error);
        throw new Error(`Failed to create person: ${response.status} - ${error}`);
      }

      const result = await response.json();
      console.log('Person created:', result);
      return result;
    } catch (error) {
      console.error('Error creating person:', error);
      throw error;
    }
  },

  updatePerson: async (id: number, person: Person): Promise<Person> => {
    try {
      console.log('Updating person:', id, person);
      const response = await fetch(`${API_URL}/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(person),
      });

      if (!response.ok) {
        const error = await response.text();
        throw new Error(`Failed to update person: ${response.status} - ${error}`);
      }

      const result = await response.json();
      console.log('Person updated:', result);
      return result;
    } catch (error) {
      console.error('Error updating person:', error);
      throw error;
    }
  },

  deletePerson: async (id: number): Promise<void> => {
    try {
      console.log('Deleting person:', id);
      const response = await fetch(`${API_URL}/${id}`, {
        method: 'DELETE',
      });

      if (!response.ok) {
        const error = await response.text();
        throw new Error(`Failed to delete person: ${response.status} - ${error}`);
      }

      console.log('Person deleted');
    } catch (error) {
      console.error('Error deleting person:', error);
      throw error;
    }
  },
};
