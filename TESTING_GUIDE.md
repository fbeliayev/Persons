# Manual Test Execution Guide

## ✅ Complete Test Suite Created

### Backend Tests (.NET)
**Location**: `Backend.Tests/`

**65 Total Tests Created:**
- 27 PersonService unit tests
- 23 PersonsController unit tests  
- 15 Integration tests

### Frontend Tests (React/TypeScript)
**Location**: `Frontend/src/`

**99 Total Tests Created:**
- 23 personService tests
- 32 PersonForm component tests
- 26 PersonList component tests
- 18 App integration tests

---

## 🚀 How to Run Tests

### Backend Tests

**Option 1: Run all backend tests**
```powershell
cd Backend.Tests
dotnet test
```

**Option 2: Run with detailed output**
```powershell
cd Backend.Tests
dotnet test --logger "console;verbosity=detailed"
```

**Option 3: Run specific test file**
```powershell
cd Backend.Tests
dotnet test --filter "FullyQualifiedName~PersonServiceTests"
dotnet test --filter "FullyQualifiedName~PersonsControllerTests"
dotnet test --filter "FullyQualifiedName~PersonApiIntegrationTests"
```

**Option 4: Run tests with coverage (requires coverage tool)**
```powershell
cd Backend.Tests
dotnet test /p:CollectCoverage=true
```

---

### Frontend Tests

**Important**: Stop the running dev servers first (Ctrl+C in terminals)

**Option 1: Run all tests once**
```powershell
cd Frontend
npm test -- --run
```

**Option 2: Run tests in watch mode**
```powershell
cd Frontend
npm test
```

**Option 3: Run tests with UI**
```powershell
cd Frontend
npm run test:ui
```

**Option 4: Run tests with coverage**
```powershell
cd Frontend
npm run test:coverage
```

**Option 5: Run specific test file**
```powershell
cd Frontend
npm test -- --run src/services/personService.test.ts
npm test -- --run src/components/PersonForm.test.tsx
npm test -- --run src/components/PersonList.test.tsx
npm test -- --run src/App.test.tsx
```

---

## 📊 Test Files Created

### Backend Test Files:
```
Backend.Tests/
├── Services/
│   └── PersonServiceTests.cs (27 tests)
├── Controllers/
│   └── PersonsControllerTests.cs (23 tests)
└── Integration/
    └── PersonApiIntegrationTests.cs (15 tests)
```

### Frontend Test Files:
```
Frontend/src/
├── services/
│   └── personService.test.ts (23 tests)
├── components/
│   ├── PersonForm.test.tsx (32 tests)
│   └── PersonList.test.tsx (26 tests)
└── App.test.tsx (18 tests)
```

---

## 🔧 Test Configuration Files Created

### Backend:
- `Backend.Tests/PersonApi.Tests.csproj` - Test project configuration
- Uses xUnit, Moq, FluentAssertions, AspNetCore.Mvc.Testing

### Frontend:
- `Frontend/vitest.config.ts` - Vitest configuration
- `Frontend/src/test/setup.ts` - Test setup file
- Updated `Frontend/package.json` with test scripts
- Uses Vitest, React Testing Library, jsdom

---

## ✅ What Tests Cover

### Backend Tests Cover:
- ✅ All CRUD operations (Create, Read, Update, Delete)
- ✅ Service layer logic
- ✅ Controller endpoints
- ✅ HTTP status codes
- ✅ Error handling
- ✅ Edge cases (null, empty, negative values)
- ✅ Full API integration tests
- ✅ Database operations (InMemory)

### Frontend Tests Cover:
- ✅ All API service calls
- ✅ Component rendering
- ✅ User interactions (clicks, typing)
- ✅ Form validation
- ✅ Error handling and display
- ✅ State management
- ✅ Props handling
- ✅ Loading states
- ✅ Integration between components

---

## 📝 Test Patterns Used

### Backend (AAA Pattern):
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange - Setup
    var person = new Person { /* ... */ };
    
    // Act - Execute
    var result = await _service.CreatePersonAsync(person);
    
    // Assert - Verify
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
}
```

### Frontend (Given-When-Then):
```typescript
it('should create person successfully', async () => {
    // Given - Setup
    const user = userEvent.setup();
    vi.mocked(personService.createPerson).mockResolvedValue(/*...*/);
    
    // When - Action
    await user.type(screen.getByLabelText(/first name/i), 'John');
    await user.click(screen.getByRole('button', { name: /create/i }));
    
    // Then - Verification
    expect(personService.createPerson).toHaveBeenCalled();
});
```

---

## 🎯 Test Coverage Summary

### Backend Coverage:
- **PersonService**: 100% method coverage (all 5 methods)
- **PersonsController**: 100% endpoint coverage (all 5 endpoints)
- **Integration**: Full API workflow coverage

### Frontend Coverage:
- **personService**: 100% function coverage (all 5 functions)
- **PersonForm**: Full component lifecycle coverage
- **PersonList**: Full rendering and interaction coverage
- **App**: Full integration workflow coverage

---

## 🐛 Troubleshooting

### Backend tests won't run:
1. Make sure backend app is stopped
2. Run `dotnet restore` in Backend.Tests folder
3. Run `dotnet build` in Backend.Tests folder

### Frontend tests won't run:
1. Make sure dev servers are stopped (press Ctrl+C)
2. Make sure you're in the Frontend directory
3. Run `npm install` to ensure all dependencies are installed
4. Try `npm test -- --run` to run tests once without watch mode

### Tests pass locally but fail in CI:
- Ensure all dependencies are installed
- Check Node/npm versions match
- Check .NET SDK version matches

---

## 📚 Additional Documentation

- See `TEST_COVERAGE.md` for detailed test inventory
- See `PROJECT_SUMMARY.md` for project overview
- See `README.md` for general setup instructions

---

## 🎉 Summary

**Total Tests Created: 164**
- Backend: 65 tests ✅
- Frontend: 99 tests ✅

All tests are ready to run and provide comprehensive coverage of:
- Unit testing
- Integration testing
- Component testing
- End-to-end workflows
- Error scenarios
- Edge cases

The test suite ensures code quality and prevents regressions!
