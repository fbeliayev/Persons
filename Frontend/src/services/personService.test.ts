import { describe, it, expect, beforeEach, vi, afterEach } from 'vitest';
import { personService } from './personService';
import type { Person } from '../types/Person';

describe('personService', () => {
  beforeEach(() => {
    // Reset fetch mock before each test
    global.fetch = vi.fn();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('getAllPersons', () => {
    it('should fetch all persons successfully', async () => {
      const mockPersons: Person[] = [
        { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 },
        { id: 2, firstName: 'Jane', lastName: 'Smith', email: 'jane@test.com', age: 25 },
      ];

      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => mockPersons,
      });

      const result = await personService.getAllPersons();

      expect(global.fetch).toHaveBeenCalledWith('http://localhost:5126/api/persons');
      expect(result).toEqual(mockPersons);
    });

    it('should throw error when fetch fails', async () => {
      (global.fetch as any).mockResolvedValueOnce({
        ok: false,
        status: 500,
        text: async () => 'Internal Server Error',
      });

      await expect(personService.getAllPersons()).rejects.toThrow();
    });

    it('should handle empty array response', async () => {
      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => [],
      });

      const result = await personService.getAllPersons();

      expect(result).toEqual([]);
    });

    it('should handle network error', async () => {
      (global.fetch as any).mockRejectedValueOnce(new Error('Network error'));

      await expect(personService.getAllPersons()).rejects.toThrow('Network error');
    });
  });

  describe('getPersonById', () => {
    it('should fetch person by id successfully', async () => {
      const mockPerson: Person = { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };

      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => mockPerson,
      });

      const result = await personService.getPersonById(1);

      expect(global.fetch).toHaveBeenCalledWith('http://localhost:5126/api/persons/1');
      expect(result).toEqual(mockPerson);
    });

    it('should throw error when person not found', async () => {
      (global.fetch as any).mockResolvedValueOnce({
        ok: false,
        status: 404,
      });

      await expect(personService.getPersonById(999)).rejects.toThrow();
    });
  });

  describe('createPerson', () => {
    it('should create person successfully', async () => {
      const newPerson: Person = { id: 0, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };
      const createdPerson: Person = { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };

      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => createdPerson,
      });

      const result = await personService.createPerson(newPerson);

      expect(global.fetch).toHaveBeenCalledWith('http://localhost:5126/api/persons', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(newPerson),
      });
      expect(result).toEqual(createdPerson);
    });

    it('should throw error when creation fails', async () => {
      const newPerson: Person = { id: 0, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };

      (global.fetch as any).mockResolvedValueOnce({
        ok: false,
        status: 400,
        text: async () => 'Bad Request',
      });

      await expect(personService.createPerson(newPerson)).rejects.toThrow();
    });

    it('should log creation attempt', async () => {
      const consoleSpy = vi.spyOn(console, 'log');
      const newPerson: Person = { id: 0, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };

      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => ({ ...newPerson, id: 1 }),
      });

      await personService.createPerson(newPerson);

      expect(consoleSpy).toHaveBeenCalledWith('Creating person:', newPerson);
      consoleSpy.mockRestore();
    });
  });

  describe('updatePerson', () => {
    it('should update person successfully', async () => {
      const updatedPerson: Person = { id: 1, firstName: 'Jane', lastName: 'Smith', email: 'jane@test.com', age: 25 };

      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => updatedPerson,
      });

      const result = await personService.updatePerson(1, updatedPerson);

      expect(global.fetch).toHaveBeenCalledWith('http://localhost:5126/api/persons/1', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(updatedPerson),
      });
      expect(result).toEqual(updatedPerson);
    });

    it('should throw error when update fails', async () => {
      const updatedPerson: Person = { id: 1, firstName: 'Jane', lastName: 'Smith', email: 'jane@test.com', age: 25 };

      (global.fetch as any).mockResolvedValueOnce({
        ok: false,
        status: 404,
        text: async () => 'Not Found',
      });

      await expect(personService.updatePerson(1, updatedPerson)).rejects.toThrow();
    });

    it('should log update attempt', async () => {
      const consoleSpy = vi.spyOn(console, 'log');
      const updatedPerson: Person = { id: 1, firstName: 'Jane', lastName: 'Smith', email: 'jane@test.com', age: 25 };

      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => updatedPerson,
      });

      await personService.updatePerson(1, updatedPerson);

      expect(consoleSpy).toHaveBeenCalledWith('Updating person:', 1, updatedPerson);
      consoleSpy.mockRestore();
    });
  });

  describe('deletePerson', () => {
    it('should delete person successfully', async () => {
      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
      });

      await personService.deletePerson(1);

      expect(global.fetch).toHaveBeenCalledWith('http://localhost:5126/api/persons/1', {
        method: 'DELETE',
      });
    });

    it('should throw error when delete fails', async () => {
      (global.fetch as any).mockResolvedValueOnce({
        ok: false,
        status: 404,
        text: async () => 'Not Found',
      });

      await expect(personService.deletePerson(999)).rejects.toThrow();
    });

    it('should log deletion attempt', async () => {
      const consoleSpy = vi.spyOn(console, 'log');

      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
      });

      await personService.deletePerson(1);

      expect(consoleSpy).toHaveBeenCalledWith('Deleting person:', 1);
      consoleSpy.mockRestore();
    });
  });

  describe('error handling', () => {
    it('should log errors for getAllPersons', async () => {
      const consoleErrorSpy = vi.spyOn(console, 'error');
      const error = new Error('Network error');

      (global.fetch as any).mockRejectedValueOnce(error);

      await expect(personService.getAllPersons()).rejects.toThrow();
      expect(consoleErrorSpy).toHaveBeenCalledWith('Error fetching persons:', error);
      
      consoleErrorSpy.mockRestore();
    });

    it('should log errors for createPerson', async () => {
      const consoleErrorSpy = vi.spyOn(console, 'error');
      const newPerson: Person = { id: 0, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };

      (global.fetch as any).mockResolvedValueOnce({
        ok: false,
        status: 400,
        text: async () => 'Bad Request',
      });

      await expect(personService.createPerson(newPerson)).rejects.toThrow();
      expect(consoleErrorSpy).toHaveBeenCalled();
      
      consoleErrorSpy.mockRestore();
    });
  });
});
