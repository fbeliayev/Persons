import { useState } from 'react';
import type { Person } from '../types/Person';

interface PersonFormProps {
  onSubmit: (person: Omit<Person, 'id'>) => void;
  editingPerson: Person | null;
  onCancel: () => void;
}

const emptyForm = {
  firstName: '',
  lastName: '',
  email: '',
  age: 0,
};

const PersonForm = ({ onSubmit, editingPerson, onCancel }: PersonFormProps) => {
  const [formData, setFormData] = useState(editingPerson || emptyForm);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
    setFormData(emptyForm);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'age' ? parseInt(value) || 0 : value,
    }));
  };

  const handleCancelClick = () => {
    setFormData(emptyForm);
    onCancel();
  };

  return (
    <div className="person-form">
      <h2>{editingPerson ? 'Edit Person' : 'Add New Person'}</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="firstName">First Name:</label>
          <input
            type="text"
            id="firstName"
            name="firstName"
            value={formData.firstName}
            onChange={handleChange}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="lastName">Last Name:</label>
          <input
            type="text"
            id="lastName"
            name="lastName"
            value={formData.lastName}
            onChange={handleChange}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="email">Email:</label>
          <input
            type="email"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="age">Age:</label>
          <input
            type="number"
            id="age"
            name="age"
            value={formData.age}
            onChange={handleChange}
            required
            min="0"
          />
        </div>
        <div className="form-actions">
          <button type="submit" className="btn btn-primary">
            {editingPerson ? 'Update' : 'Create'}
          </button>
          {editingPerson && (
            <button type="button" className="btn btn-secondary" onClick={handleCancelClick}>
              Cancel
            </button>
          )}
        </div>
      </form>
    </div>
  );
};

export default PersonForm;
