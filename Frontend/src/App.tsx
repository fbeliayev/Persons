import { useState, useEffect } from 'react'
import './App.css'
import PersonList from './components/PersonList'
import PersonForm from './components/PersonForm'
import PersonDetails from './components/PersonDetails'
import type { Person } from './types/Person'
import { personService } from './services/personService'

function App() {
  const [persons, setPersons] = useState<Person[]>([])
  const [editingPerson, setEditingPerson] = useState<Person | null>(null)
  const [viewingPerson, setViewingPerson] = useState<Person | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    loadPersons()
  }, [])

  const loadPersons = async () => {
    setLoading(true)
    setError(null)
    try {
      const data = await personService.getAllPersons()
      setPersons(data)
      console.log('Loaded persons:', data)
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to load persons'
      console.error('Failed to load persons:', error)
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  const handleCreateOrUpdate = async (person: Omit<Person, 'id'>) => {
    setError(null)
    try {
      if (editingPerson) {
        console.log('Updating person:', editingPerson.id, person)
        await personService.updatePerson(editingPerson.id, { ...person, id: editingPerson.id } as Person)
      } else {
        console.log('Creating new person:', person)
        await personService.createPerson({ ...person, id: 0 } as Person)
      }
      await loadPersons()
      setEditingPerson(null)
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to save person'
      console.error('Failed to save person:', error)
      setError(errorMessage)
      alert(`Error: ${errorMessage}`)
    }
  }

  const handleEdit = (person: Person) => {
    setEditingPerson(person)
    setError(null)
  }

  const handleDelete = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this person?')) {
      setError(null)
      try {
        await personService.deletePerson(id)
        await loadPersons()
      } catch (error) {
        const errorMessage = error instanceof Error ? error.message : 'Failed to delete person'
        console.error('Failed to delete person:', error)
        setError(errorMessage)
        alert(`Error: ${errorMessage}`)
      }
    }
  }

  const handleCancelEdit = () => {
    setEditingPerson(null)
    setError(null)
  }

  const handleViewCities = (person: Person) => {
    setViewingPerson(person)
  }

  const handleCloseCities = async () => {
    setViewingPerson(null)
    // Refresh the persons list to update cities in the table
    await loadPersons()
  }

  return (
    <div className="app">
      <h1>Person Management</h1>
      {error && (
        <div className="error-message">
          {error}
        </div>
      )}
      <div className="container">
        <PersonForm
          key={editingPerson?.id || 'new'}
          onSubmit={handleCreateOrUpdate}
          editingPerson={editingPerson}
          onCancel={handleCancelEdit}
        />
        <PersonList
          persons={persons}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onViewCities={handleViewCities}
          loading={loading}
        />
      </div>
      {viewingPerson && (
        <PersonDetails
          person={viewingPerson}
          onClose={handleCloseCities}
        />
      )}
    </div>
  )
}

export default App
