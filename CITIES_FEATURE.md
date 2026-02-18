# Cities to Visit Feature

## Overview
Added a new feature that allows persons to manage their list of cities to visit, with the ability to mark cities as visited and track visit dates.

## Backend Changes

### New Models
- **City.cs**: Represents a city with Id, Name, and Country
- **PersonCity.cs**: Junction table for many-to-many relationship with IsVisited flag and VisitedDate

### Database
- Seeded 10 cities: Paris, Tokyo, New York, Barcelona, Dubai, London, Rome, Sydney, Istanbul, Amsterdam
- Added PersonCities many-to-many relationship table

### New Services
- **ICityService / CityService**: Manages city operations
  - GetAllCitiesAsync()
  - GetCityByIdAsync(id)

### Updated PersonService
Added methods for managing person-city relationships:
- GetPersonCitiesAsync(personId)
- AddCityToPersonAsync(personId, cityId)
- MarkCityAsVisitedAsync(personId, cityId, isVisited, visitedDate)
- RemoveCityFromPersonAsync(personId, cityId)

### New Controllers
- **CitiesController**: GET endpoints for cities
- **PersonsController**: Added city management endpoints
  - GET `/api/persons/{id}/cities` - Get all cities for a person
  - POST `/api/persons/{id}/cities/{cityId}` - Add city to person
  - PUT `/api/persons/{id}/cities/{cityId}` - Update visited status
  - DELETE `/api/persons/{id}/cities/{cityId}` - Remove city from person

### Configuration
- Added JSON serialization options to handle circular references
- Configured ReferenceHandler.IgnoreCycles

## Frontend Changes

### New Types
- **City.ts**: City and PersonCity interfaces

### New Services
- **cityService.ts**: API client for city operations

### New Components
- **PersonDetails.tsx**: Modal component for managing person's cities
  - Multiselect dropdown for adding cities
  - Grid display of added cities
  - Mark as visited functionality
  - Remove city functionality
  - Shows visited date when marked as visited

### Updated Components
- **PersonList.tsx**: Added "🌍 Cities" button to view/manage cities
- **App.tsx**: Integrated PersonDetails modal

### Styling
- **PersonDetails.css**: Complete styling for the cities modal
- Updated App.css with btn-info style

## How to Use

### Backend
```bash
cd Backend
dotnet run
```
Backend runs on http://localhost:5126

### Frontend
```bash
cd Frontend
npm install
npm run dev
```
Frontend typically runs on http://localhost:5173

## Features
1. **View Cities**: Click "🌍 Cities" button on any person to see their city list
2. **Add Cities**: Select multiple cities from the dropdown (Hold Ctrl/Cmd to select multiple) and click "Add Selected Cities"
3. **Mark as Visited**: Click "Mark as Visited" button on any city card
4. **Track Visit Date**: When marked as visited, the current date is automatically recorded
5. **Remove Cities**: Click "Remove" to remove a city from the person's list
6. **Visual Indicators**: Visited cities are highlighted with green border and background

## API Endpoints

### Cities
- GET `/api/cities` - Get all available cities
- GET `/api/cities/{id}` - Get specific city

### Person Cities
- GET `/api/persons/{personId}/cities` - Get person's cities
- POST `/api/persons/{personId}/cities/{cityId}` - Add city to person
- PUT `/api/persons/{personId}/cities/{cityId}` - Update visited status
  ```json
  {
    "isVisited": true,
    "visitedDate": "2024-01-15T00:00:00Z"
  }
  ```
- DELETE `/api/persons/{personId}/cities/{cityId}` - Remove city from person

## Testing
All existing tests continue to pass. Integration tests use isolated databases to ensure test independence.

```bash
dotnet test
```
