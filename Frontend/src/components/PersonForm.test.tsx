import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import PersonForm from './PersonForm';
import type { Person } from '../types/Person';

describe('PersonForm', () => {
  const mockOnSubmit = vi.fn();
  const mockOnCancel = vi.fn();

  beforeEach(() => {
    mockOnSubmit.mockClear();
    mockOnCancel.mockClear();
  });

  describe('rendering', () => {
    it('should render form with all fields', () => {
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/first name/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/last name/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/age/i)).toBeInTheDocument();
    });

    it('should show "Add New Person" title when not editing', () => {
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      expect(screen.getByText('Add New Person')).toBeInTheDocument();
    });

    it('should show "Edit Person" title when editing', () => {
      const person: Person = { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };
      
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={person} onCancel={mockOnCancel} />);

      expect(screen.getByText('Edit Person')).toBeInTheDocument();
    });

    it('should show Create button when not editing', () => {
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      expect(screen.getByRole('button', { name: /create/i })).toBeInTheDocument();
    });

    it('should show Update button when editing', () => {
      const person: Person = { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };
      
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={person} onCancel={mockOnCancel} />);

      expect(screen.getByRole('button', { name: /update/i })).toBeInTheDocument();
    });

    it('should show Cancel button when editing', () => {
      const person: Person = { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };
      
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={person} onCancel={mockOnCancel} />);

      expect(screen.getByRole('button', { name: /cancel/i })).toBeInTheDocument();
    });

    it('should not show Cancel button when not editing', () => {
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      expect(screen.queryByRole('button', { name: /cancel/i })).not.toBeInTheDocument();
    });
  });

  describe('form input', () => {
    it('should update first name field', async () => {
      const user = userEvent.setup();
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      const firstNameInput = screen.getByLabelText(/first name/i);
      await user.type(firstNameInput, 'John');

      expect(firstNameInput).toHaveValue('John');
    });

    it('should update last name field', async () => {
      const user = userEvent.setup();
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      const lastNameInput = screen.getByLabelText(/last name/i);
      await user.type(lastNameInput, 'Doe');

      expect(lastNameInput).toHaveValue('Doe');
    });

    it('should update email field', async () => {
      const user = userEvent.setup();
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      const emailInput = screen.getByLabelText(/email/i);
      await user.type(emailInput, 'john@test.com');

      expect(emailInput).toHaveValue('john@test.com');
    });

    it('should update age field', async () => {
      const user = userEvent.setup();
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      const ageInput = screen.getByLabelText(/age/i);
      await user.clear(ageInput);
      await user.type(ageInput, '30');

      expect(ageInput).toHaveValue(30);
    });

    it('should handle age field with non-numeric input', async () => {
      const user = userEvent.setup();
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      const ageInput = screen.getByLabelText(/age/i);
      await user.clear(ageInput);
      await user.type(ageInput, 'abc');

      expect(ageInput).toHaveValue(0);
    });
  });

  describe('form submission', () => {
    it('should submit form with correct data', async () => {
      const user = userEvent.setup();
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      await user.type(screen.getByLabelText(/first name/i), 'John');
      await user.type(screen.getByLabelText(/last name/i), 'Doe');
      await user.type(screen.getByLabelText(/email/i), 'john@test.com');
      const ageInput = screen.getByLabelText(/age/i);
      await user.clear(ageInput);
      await user.type(ageInput, '30');

      await user.click(screen.getByRole('button', { name: /create/i }));

      expect(mockOnSubmit).toHaveBeenCalledWith({
        firstName: 'John',
        lastName: 'Doe',
        email: 'john@test.com',
        age: 30,
      });
    });

    it('should clear form after submission', async () => {
      const user = userEvent.setup();
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      await user.type(screen.getByLabelText(/first name/i), 'John');
      await user.type(screen.getByLabelText(/last name/i), 'Doe');
      await user.type(screen.getByLabelText(/email/i), 'john@test.com');

      await user.click(screen.getByRole('button', { name: /create/i }));

      expect(screen.getByLabelText(/first name/i)).toHaveValue('');
      expect(screen.getByLabelText(/last name/i)).toHaveValue('');
      expect(screen.getByLabelText(/email/i)).toHaveValue('');
    });

    it('should prevent default form submission', async () => {
      const user = userEvent.setup();
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      const form = screen.getByRole('button', { name: /create/i }).closest('form')!;
      const submitEvent = new Event('submit', { bubbles: true, cancelable: true });
      const preventDefaultSpy = vi.spyOn(submitEvent, 'preventDefault');

      fireEvent(form, submitEvent);

      expect(preventDefaultSpy).toHaveBeenCalled();
    });
  });

  describe('editing mode', () => {
    it('should populate form with editing person data', () => {
      const person: Person = { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };
      
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={person} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/first name/i)).toHaveValue('John');
      expect(screen.getByLabelText(/last name/i)).toHaveValue('Doe');
      expect(screen.getByLabelText(/email/i)).toHaveValue('john@test.com');
      expect(screen.getByLabelText(/age/i)).toHaveValue(30);
    });

    it('should submit updated person data', async () => {
      const user = userEvent.setup();
      const person: Person = { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };
      
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={person} onCancel={mockOnCancel} />);

      await user.clear(screen.getByLabelText(/first name/i));
      await user.type(screen.getByLabelText(/first name/i), 'Jane');

      await user.click(screen.getByRole('button', { name: /update/i }));

      expect(mockOnSubmit).toHaveBeenCalledWith({
        firstName: 'Jane',
        lastName: 'Doe',
        email: 'john@test.com',
        age: 30,
      });
    });

    it('should call onCancel when cancel button is clicked', async () => {
      const user = userEvent.setup();
      const person: Person = { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };
      
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={person} onCancel={mockOnCancel} />);

      await user.click(screen.getByRole('button', { name: /cancel/i }));

      expect(mockOnCancel).toHaveBeenCalledTimes(1);
    });

    it('should reset form when cancel is clicked', async () => {
      const user = userEvent.setup();
      const person: Person = { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };
      
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={person} onCancel={mockOnCancel} />);

      await user.type(screen.getByLabelText(/first name/i), ' Modified');
      await user.click(screen.getByRole('button', { name: /cancel/i }));

      expect(screen.getByLabelText(/first name/i)).toHaveValue('');
    });
  });

  describe('validation', () => {
    it('should have required attribute on first name', () => {
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/first name/i)).toBeRequired();
    });

    it('should have required attribute on last name', () => {
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/last name/i)).toBeRequired();
    });

    it('should have required attribute on email', () => {
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/email/i)).toBeRequired();
    });

    it('should have required attribute on age', () => {
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/age/i)).toBeRequired();
    });

    it('should have min attribute set to 0 on age', () => {
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/age/i)).toHaveAttribute('min', '0');
    });

    it('should have type email on email field', () => {
      render(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/email/i)).toHaveAttribute('type', 'email');
    });
  });

  describe('component re-rendering', () => {
    it('should update form when editingPerson prop changes', () => {
      const person1: Person = { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };
      const person2: Person = { id: 2, firstName: 'Jane', lastName: 'Smith', email: 'jane@test.com', age: 25 };
      
      const { rerender } = render(<PersonForm onSubmit={mockOnSubmit} editingPerson={person1} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/first name/i)).toHaveValue('John');

      rerender(<PersonForm onSubmit={mockOnSubmit} editingPerson={person2} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/first name/i)).toHaveValue('Jane');
    });

    it('should clear form when editingPerson changes from person to null', () => {
      const person: Person = { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@test.com', age: 30 };
      
      const { rerender } = render(<PersonForm onSubmit={mockOnSubmit} editingPerson={person} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/first name/i)).toHaveValue('John');

      rerender(<PersonForm onSubmit={mockOnSubmit} editingPerson={null} onCancel={mockOnCancel} />);

      expect(screen.getByLabelText(/first name/i)).toHaveValue('');
    });
  });
});
