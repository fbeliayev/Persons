# Person Management Application

A full-stack application with .NET backend and React frontend for managing persons with CRUD operations.

## Project Structure

```
├── Backend/          # .NET Web API
│   ├── Controllers/  # API Controllers
│   ├── Models/       # Entity models
│   ├── Services/     # Business logic services
│   └── Data/         # Database context
└── Frontend/         # React TypeScript application
    └── src/
        ├── components/   # React components
        ├── services/     # API service layer
        └── types/        # TypeScript types
```

## Backend (.NET Web API)

### Features
- RESTful API with CRUD operations for Person entity
- In-memory database (Entity Framework Core)
- Service layer for business logic
- CORS enabled for React frontend

### Person Entity
- Id (int)
- FirstName (string)
- LastName (string)
- Email (string)
- Age (int)

### API Endpoints
- `GET /api/persons` - Get all persons
- `GET /api/persons/{id}` - Get person by ID
- `POST /api/persons` - Create new person
- `PUT /api/persons/{id}` - Update existing person
- `DELETE /api/persons/{id}` - Delete person

### Running the Backend

1. Navigate to the Backend directory:
   ```bash
   cd Backend
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. The API will be available at `http://localhost:5000` and `https://localhost:5001`

## Frontend (React + TypeScript)

### Features
- List of all persons in a table
- Add new person form
- Edit existing person (inline form update)
- Delete person with confirmation
- Responsive design

### Components
- **App.tsx** - Main application component with state management
- **PersonList.tsx** - Displays persons in a table with edit/delete actions
- **PersonForm.tsx** - Form for creating and updating persons

### Running the Frontend

1. Navigate to the Frontend directory:
   ```bash
   cd Frontend
   ```

2. Install dependencies (if not already done):
   ```bash
   npm install
   ```

3. Run the development server:
   ```bash
   npm run dev
   ```

4. The application will be available at `http://localhost:5173`

## Running Both Together

### Option 1: Using separate terminals

**Terminal 1 (Backend):**
```bash
cd Backend
dotnet run
```

**Terminal 2 (Frontend):**
```bash
cd Frontend
npm run dev
```

### Option 2: Using PowerShell script

Create a file named `start.ps1` in the root directory:
```powershell
# Start backend
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd Backend; dotnet run"

# Wait a bit for backend to start
Start-Sleep -Seconds 5

# Start frontend
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd Frontend; npm run dev"
```

Run: `.\start.ps1`

## Testing the Application

1. Start both backend and frontend
2. Open your browser to `http://localhost:5173`
3. Add a new person using the form
4. View the person in the list
5. Edit the person by clicking "Edit"
6. Delete the person by clicking "Delete"

## Technologies Used

### Backend
- .NET 10.0
- ASP.NET Core Web API
- Entity Framework Core (In-Memory)
- C# 13

### Frontend
- React 19
- TypeScript
- Vite
- CSS3

## Notes

- The backend uses an in-memory database, so data will be lost when the application stops
- CORS is configured to allow requests from `http://localhost:3000` and `http://localhost:5173`
- The API runs on port 5000 (HTTP) and 5001 (HTTPS) by default
- The React app runs on port 5173 by default (Vite)
