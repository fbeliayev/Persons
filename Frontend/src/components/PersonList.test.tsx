import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import PersonList from './PersonList';
import type { Person } from '../types/Person';

describe('PersonList', () => {
  const mockOnEdit = vi.fn();
  const mockOnDelete = vi.fn();

  const mockPersons: Person[] = [
    { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 },
    { id: 2, firstName: 'Jane', lastName: 'Smith', email: 'jane@test.com', age: 25 },
    { id: 3, firstName: 'Bob', lastName: 'Johnson', email: 'bob@test.com', age: 35 },
  ];

  beforeEach(() => {
    mockOnEdit.mockClear();
    mockOnDelete.mockClear();
  });

  describe('rendering', () => {
    it('should show loading message when loading is true', () => {
      render(<PersonList persons={[]} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={true} />);

      expect(screen.getByText(/loading persons/i)).toBeInTheDocument();
    });

    it('should show empty message when no persons and not loading', () => {
      render(<PersonList persons={[]} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.getByText(/no persons found/i)).toBeInTheDocument();
    });

    it('should render table with headers when persons exist', () => {
      render(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.getByText('First Name')).toBeInTheDocument();
      expect(screen.getByText('Last Name')).toBeInTheDocument();
      expect(screen.getByText('Email')).toBeInTheDocument();
      expect(screen.getByText('Age')).toBeInTheDocument();
      expect(screen.getByText('Actions')).toBeInTheDocument();
    });

    it('should render all persons in the table', () => {
      render(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.getByText('John')).toBeInTheDocument();
      expect(screen.getByText('Jane')).toBeInTheDocument();
      expect(screen.getByText('Bob')).toBeInTheDocument();
    });

    it('should display person email addresses', () => {
      render(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.getByText('john@test.com')).toBeInTheDocument();
      expect(screen.getByText('jane@test.com')).toBeInTheDocument();
      expect(screen.getByText('bob@test.com')).toBeInTheDocument();
    });

    it('should display person ages', () => {
      render(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.getByText('30')).toBeInTheDocument();
      expect(screen.getByText('25')).toBeInTheDocument();
      expect(screen.getByText('35')).toBeInTheDocument();
    });

    it('should render Edit and Delete buttons for each person', () => {
      render(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      const editButtons = screen.getAllByRole('button', { name: /edit/i });
      const deleteButtons = screen.getAllByRole('button', { name: /delete/i });

      expect(editButtons).toHaveLength(3);
      expect(deleteButtons).toHaveLength(3);
    });
  });

  describe('edit functionality', () => {
    it('should call onEdit with correct person when Edit button clicked', async () => {
      const user = userEvent.setup();
      render(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      const editButtons = screen.getAllByRole('button', { name: /edit/i });
      await user.click(editButtons[0]);

      expect(mockOnEdit).toHaveBeenCalledWith(mockPersons[0]);
      expect(mockOnEdit).toHaveBeenCalledTimes(1);
    });

    it('should call onEdit with correct person for different rows', async () => {
      const user = userEvent.setup();
      render(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      const editButtons = screen.getAllByRole('button', { name: /edit/i });
      
      await user.click(editButtons[1]);
      expect(mockOnEdit).toHaveBeenCalledWith(mockPersons[1]);

      await user.click(editButtons[2]);
      expect(mockOnEdit).toHaveBeenCalledWith(mockPersons[2]);
    });
  });

  describe('delete functionality', () => {
    it('should call onDelete with correct person id when Delete button clicked', async () => {
      const user = userEvent.setup();
      render(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      const deleteButtons = screen.getAllByRole('button', { name: /delete/i });
      await user.click(deleteButtons[0]);

      expect(mockOnDelete).toHaveBeenCalledWith(1);
      expect(mockOnDelete).toHaveBeenCalledTimes(1);
    });

    it('should call onDelete with correct id for different rows', async () => {
      const user = userEvent.setup();
      render(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      const deleteButtons = screen.getAllByRole('button', { name: /delete/i });
      
      await user.click(deleteButtons[1]);
      expect(mockOnDelete).toHaveBeenCalledWith(2);

      await user.click(deleteButtons[2]);
      expect(mockOnDelete).toHaveBeenCalledWith(3);
    });
  });

  describe('edge cases', () => {
    it('should render single person correctly', () => {
      const singlePerson = [mockPersons[0]];
      render(<PersonList persons={singlePerson} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.getByText('John')).toBeInTheDocument();
      expect(screen.getAllByRole('button', { name: /edit/i })).toHaveLength(1);
      expect(screen.getAllByRole('button', { name: /delete/i })).toHaveLength(1);
    });

    it('should handle person with empty strings', () => {
      const personWithEmptyStrings: Person[] = [
        { id: 1, firstName: '', lastName: '', email: '', age: 0 },
      ];

      render(<PersonList persons={personWithEmptyStrings} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.getByRole('table')).toBeInTheDocument();
    });

    it('should handle person with special characters in name', () => {
      const personWithSpecialChars: Person[] = [
        { id: 1, firstName: 'José', lastName: "O'Brien", email: 'jose@test.com', age: 30 },
      ];

      render(<PersonList persons={personWithSpecialChars} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.getByText('José')).toBeInTheDocument();
      expect(screen.getByText("O'Brien")).toBeInTheDocument();
    });

    it('should handle person with very long names', () => {
      const personWithLongName: Person[] = [
        { id: 1, firstName: 'VeryLongFirstNameThatGoesOnForever', lastName: 'VeryLongLastNameThatGoesOnForever', email: 'long@test.com', age: 30 },
      ];

      render(<PersonList persons={personWithLongName} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.getByText('VeryLongFirstNameThatGoesOnForever')).toBeInTheDocument();
    });

    it('should handle very large age numbers', () => {
      const personWithLargeAge: Person[] = [
        { id: 1, firstName: 'Old', lastName: 'Person', email: 'old@test.com', age: 999999 },
      ];

      render(<PersonList persons={personWithLargeAge} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.getByText('999999')).toBeInTheDocument();
    });
  });

  describe('component state changes', () => {
    it('should transition from loading to showing data', () => {
      const { rerender } = render(
        <PersonList persons={[]} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={true} />
      );

      expect(screen.getByText(/loading persons/i)).toBeInTheDocument();

      rerender(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.queryByText(/loading persons/i)).not.toBeInTheDocument();
      expect(screen.getByText('John')).toBeInTheDocument();
    });

    it('should update when persons list changes', () => {
      const initialPersons = [mockPersons[0]];
      const updatedPersons = mockPersons;

      const { rerender } = render(
        <PersonList persons={initialPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />
      );

      expect(screen.getAllByRole('row')).toHaveLength(2); // header + 1 data row

      rerender(<PersonList persons={updatedPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.getAllByRole('row')).toHaveLength(4); // header + 3 data rows
    });

    it('should transition from data to empty state', () => {
      const { rerender } = render(
        <PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />
      );

      expect(screen.getByRole('table')).toBeInTheDocument();

      rerender(<PersonList persons={[]} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      expect(screen.queryByRole('table')).not.toBeInTheDocument();
      expect(screen.getByText(/no persons found/i)).toBeInTheDocument();
    });
  });

  describe('accessibility', () => {
    it('should render table with proper structure', () => {
      render(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      const table = screen.getByRole('table');
      expect(table).toBeInTheDocument();
    });

    it('should have proper button roles', () => {
      render(<PersonList persons={mockPersons} onEdit={mockOnEdit} onDelete={mockOnDelete} loading={false} />);

      const editButtons = screen.getAllByRole('button', { name: /edit/i });
      const deleteButtons = screen.getAllByRole('button', { name: /delete/i });

      editButtons.forEach(button => {
        expect(button).toHaveAttribute('type', 'button');
      });

      deleteButtons.forEach(button => {
        expect(button).toHaveAttribute('type', 'button');
      });
    });
  });
});
