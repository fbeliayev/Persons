import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import App from './App';
import { personService } from './services/personService';
import type { Person } from './types/Person';

vi.mock('./services/personService');

describe('App', () => {
  const mockPersons: Person[] = [
    { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 },
    { id: 2, firstName: 'Jane', lastName: 'Smith', email: 'jane@test.com', age: 25 },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(personService.getAllPersons).mockResolvedValue([]);
    window.confirm = vi.fn(() => true);
    window.alert = vi.fn();
  });

  describe('initial rendering', () => {
    it('should render main title', () => {
      render(<App />);

      expect(screen.getByText('Person Management')).toBeInTheDocument();
    });

    it('should load persons on mount', async () => {
      vi.mocked(personService.getAllPersons).mockResolvedValue(mockPersons);

      render(<App />);

      await waitFor(() => {
        expect(personService.getAllPersons).toHaveBeenCalledTimes(1);
      });
    });

    it('should display persons after loading', async () => {
      vi.mocked(personService.getAllPersons).mockResolvedValue(mockPersons);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText('John')).toBeInTheDocument();
        expect(screen.getByText('Jane')).toBeInTheDocument();
      });
    });

    it('should show loading state initially', () => {
      vi.mocked(personService.getAllPersons).mockImplementation(
        () => new Promise(() => {}) // Never resolves
      );

      render(<App />);

      expect(screen.getByText(/loading persons/i)).toBeInTheDocument();
    });
  });

  describe('create person', () => {
    it('should create a new person successfully', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons).mockResolvedValue([]);
      vi.mocked(personService.createPerson).mockResolvedValue({
        id: 1,
        firstName: 'John',
        lastName: 'Doe',
        email: 'john@test.com',
        age: 30,
      });

      render(<App />);

      await waitFor(() => {
        expect(screen.getByLabelText(/first name/i)).toBeInTheDocument();
      });

      await user.type(screen.getByLabelText(/first name/i), 'John');
      await user.type(screen.getByLabelText(/last name/i), 'Doe');
      await user.type(screen.getByLabelText(/email/i), 'john@test.com');
      const ageInput = screen.getByLabelText(/age/i);
      await user.clear(ageInput);
      await user.type(ageInput, '30');

      await user.click(screen.getByRole('button', { name: /create/i }));

      await waitFor(() => {
        expect(personService.createPerson).toHaveBeenCalledWith({
          firstName: 'John',
          lastName: 'Doe',
          email: 'john@test.com',
          age: 30,
          id: 0,
        });
      });
    });

    it('should reload persons after creation', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons)
        .mockResolvedValueOnce([])
        .mockResolvedValueOnce(mockPersons);
      vi.mocked(personService.createPerson).mockResolvedValue(mockPersons[0]);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByLabelText(/first name/i)).toBeInTheDocument();
      });

      await user.type(screen.getByLabelText(/first name/i), 'John');
      await user.type(screen.getByLabelText(/last name/i), 'Doe');
      await user.type(screen.getByLabelText(/email/i), 'john@test.com');
      const ageInput = screen.getByLabelText(/age/i);
      await user.clear(ageInput);
      await user.type(ageInput, '30');

      await user.click(screen.getByRole('button', { name: /create/i }));

      await waitFor(() => {
        expect(personService.getAllPersons).toHaveBeenCalledTimes(2);
      });
    });

    it('should show error message when creation fails', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons).mockResolvedValue([]);
      vi.mocked(personService.createPerson).mockRejectedValue(new Error('Creation failed'));

      render(<App />);

      await waitFor(() => {
        expect(screen.getByLabelText(/first name/i)).toBeInTheDocument();
      });

      await user.type(screen.getByLabelText(/first name/i), 'John');
      await user.type(screen.getByLabelText(/last name/i), 'Doe');
      await user.type(screen.getByLabelText(/email/i), 'john@test.com');

      await user.click(screen.getByRole('button', { name: /create/i }));

      await waitFor(() => {
        expect(screen.getByText(/Creation failed/i)).toBeInTheDocument();
      });
    });
  });

  describe('edit person', () => {
    it('should populate form when edit button clicked', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons).mockResolvedValue(mockPersons);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText('John')).toBeInTheDocument();
      });

      const editButtons = screen.getAllByRole('button', { name: /edit/i });
      await user.click(editButtons[0]);

      expect(screen.getByLabelText(/first name/i)).toHaveValue('John');
      expect(screen.getByLabelText(/last name/i)).toHaveValue('Doe');
      expect(screen.getByLabelText(/email/i)).toHaveValue('john@test.com');
      expect(screen.getByLabelText(/age/i)).toHaveValue(30);
    });

    it('should update person successfully', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons).mockResolvedValue(mockPersons);
      vi.mocked(personService.updatePerson).mockResolvedValue({
        ...mockPersons[0],
        firstName: 'Jane',
      });

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText('John')).toBeInTheDocument();
      });

      const editButtons = screen.getAllByRole('button', { name: /edit/i });
      await user.click(editButtons[0]);

      const firstNameInput = screen.getByLabelText(/first name/i);
      await user.clear(firstNameInput);
      await user.type(firstNameInput, 'Jane');

      await user.click(screen.getByRole('button', { name: /update/i }));

      await waitFor(() => {
        expect(personService.updatePerson).toHaveBeenCalledWith(1, {
          id: 1,
          firstName: 'Jane',
          lastName: 'Doe',
          email: 'john@test.com',
          age: 30,
        });
      });
    });

    it('should show Update button when editing', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons).mockResolvedValue(mockPersons);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText('John')).toBeInTheDocument();
      });

      const editButtons = screen.getAllByRole('button', { name: /edit/i });
      await user.click(editButtons[0]);

      expect(screen.getByRole('button', { name: /update/i })).toBeInTheDocument();
      expect(screen.queryByRole('button', { name: /^create$/i })).not.toBeInTheDocument();
    });

    it('should cancel editing when cancel button clicked', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons).mockResolvedValue(mockPersons);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText('John')).toBeInTheDocument();
      });

      const editButtons = screen.getAllByRole('button', { name: /edit/i });
      await user.click(editButtons[0]);

      await user.click(screen.getByRole('button', { name: /cancel/i }));

      expect(screen.getByLabelText(/first name/i)).toHaveValue('');
      expect(screen.getByRole('button', { name: /create/i })).toBeInTheDocument();
    });
  });

  describe('delete person', () => {
    it('should show confirmation dialog before delete', async () => {
      const user = userEvent.setup();
      const confirmSpy = vi.spyOn(window, 'confirm');
      vi.mocked(personService.getAllPersons).mockResolvedValue(mockPersons);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText('John')).toBeInTheDocument();
      });

      const deleteButtons = screen.getAllByRole('button', { name: /delete/i });
      await user.click(deleteButtons[0]);

      expect(confirmSpy).toHaveBeenCalledWith('Are you sure you want to delete this person?');
    });

    it('should delete person when confirmed', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons).mockResolvedValue(mockPersons);
      vi.mocked(personService.deletePerson).mockResolvedValue();
      window.confirm = vi.fn(() => true);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText('John')).toBeInTheDocument();
      });

      const deleteButtons = screen.getAllByRole('button', { name: /delete/i });
      await user.click(deleteButtons[0]);

      await waitFor(() => {
        expect(personService.deletePerson).toHaveBeenCalledWith(1);
      });
    });

    it('should not delete person when cancelled', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons).mockResolvedValue(mockPersons);
      window.confirm = vi.fn(() => false);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText('John')).toBeInTheDocument();
      });

      const deleteButtons = screen.getAllByRole('button', { name: /delete/i });
      await user.click(deleteButtons[0]);

      expect(personService.deletePerson).not.toHaveBeenCalled();
    });

    it('should reload persons after deletion', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons)
        .mockResolvedValueOnce(mockPersons)
        .mockResolvedValueOnce([mockPersons[1]]);
      vi.mocked(personService.deletePerson).mockResolvedValue();
      window.confirm = vi.fn(() => true);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText('John')).toBeInTheDocument();
      });

      const deleteButtons = screen.getAllByRole('button', { name: /delete/i });
      await user.click(deleteButtons[0]);

      await waitFor(() => {
        expect(personService.getAllPersons).toHaveBeenCalledTimes(2);
      });
    });

    it('should show error message when deletion fails', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons).mockResolvedValue(mockPersons);
      vi.mocked(personService.deletePerson).mockRejectedValue(new Error('Deletion failed'));
      window.confirm = vi.fn(() => true);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText('John')).toBeInTheDocument();
      });

      const deleteButtons = screen.getAllByRole('button', { name: /delete/i });
      await user.click(deleteButtons[0]);

      await waitFor(() => {
        expect(screen.getByText(/Deletion failed/i)).toBeInTheDocument();
      });
    });
  });

  describe('error handling', () => {
    it('should display error message when loading fails', async () => {
      vi.mocked(personService.getAllPersons).mockRejectedValue(new Error('Network error'));

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText(/Network error/i)).toBeInTheDocument();
      });
    });

    it('should clear error message after successful operation', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons)
        .mockRejectedValueOnce(new Error('Load error'))
        .mockResolvedValueOnce([]);
      vi.mocked(personService.createPerson).mockResolvedValue(mockPersons[0]);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText(/Load error/i)).toBeInTheDocument();
      });

      await user.type(screen.getByLabelText(/first name/i), 'John');
      await user.type(screen.getByLabelText(/last name/i), 'Doe');
      await user.type(screen.getByLabelText(/email/i), 'john@test.com');

      await user.click(screen.getByRole('button', { name: /create/i }));

      await waitFor(() => {
        expect(screen.queryByText(/Load error/i)).not.toBeInTheDocument();
      });
    });
  });

  describe('component integration', () => {
    it('should have PersonForm and PersonList components', () => {
      render(<App />);

      expect(screen.getByLabelText(/first name/i)).toBeInTheDocument();
      expect(screen.getByText(/Persons List|No persons found/i)).toBeInTheDocument();
    });

    it('should coordinate between form and list', async () => {
      const user = userEvent.setup();
      vi.mocked(personService.getAllPersons)
        .mockResolvedValueOnce([])
        .mockResolvedValueOnce(mockPersons);
      vi.mocked(personService.createPerson).mockResolvedValue(mockPersons[0]);

      render(<App />);

      await waitFor(() => {
        expect(screen.getByText(/no persons found/i)).toBeInTheDocument();
      });

      await user.type(screen.getByLabelText(/first name/i), 'John');
      await user.type(screen.getByLabelText(/last name/i), 'Doe');
      await user.type(screen.getByLabelText(/email/i), 'john@test.com');

      await user.click(screen.getByRole('button', { name: /create/i }));

      await waitFor(() => {
        expect(screen.getByText('John')).toBeInTheDocument();
      });
    });
  });
});
