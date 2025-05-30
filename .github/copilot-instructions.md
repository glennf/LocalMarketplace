# LocalMarketplace Copilot Instructions

## Project Overview

LocalMarketplace is a cross-platform mobile application built with .NET MAUI that facilitates buying and selling items locally with map-based discovery features.

## Architecture Guidelines

### MVVM Pattern
- **Strictly follow MVVM architecture** for all components
- **ViewModels** should contain all business logic and data binding
- **Views** should have minimal code-behind
- **Models** should be plain data objects with minimal logic

### Example ViewModel Structure:
```csharp
public class ItemViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    
    public ItemViewModel(IItemService itemService)
    {
        _itemService = itemService;
        LoadItemsCommand = new AsyncRelayCommand(LoadItemsAsync);
    }
    
    private ObservableCollection<Item> _items = new();
    public ObservableCollection<Item> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }
    
    public IAsyncRelayCommand LoadItemsCommand { get; }
    
    private async Task LoadItemsAsync()
    {
        try
        {
            var items = await _itemService.GetItemsAsync();
            Items = new ObservableCollection<Item>(items);
        }
        catch (Exception ex)
        {
            // Error handling
        }
    }
}
```

## Coding Standards

### Naming Conventions
- Use **PascalCase** for class names, public properties, and methods
- Use **camelCase** for private fields and local variables
- Prefix interfaces with "I" (e.g., `IDataService`)
- Prefix private fields with underscore (e.g., `_itemService`)

### Documentation
- Add XML documentation to all public methods and classes
- Document parameters and return values
- Include `<example>` tags for complex methods

```csharp
/// <summary>
/// Retrieves items within the specified radius of a location.
/// </summary>
/// <param name="latitude">The latitude of the center point.</param>
/// <param name="longitude">The longitude of the center point.</param>
/// <param name="radiusKm">The search radius in kilometers.</param>
/// <returns>A collection of items within the specified radius.</returns>
public async Task<IEnumerable<Item>> GetItemsByLocationAsync(double latitude, double longitude, double radiusKm)
```

### Code Organization
- Group related files in appropriate folders (`Models`, `ViewModels`, `Views`, `Services`)
- One class per file
- Keep methods short and focused (< 30 lines)
- Follow the Single Responsibility Principle

## Test-Driven Development Requirements

### General Testing Principles
- Write tests BEFORE implementation code
- Follow Red-Green-Refactor workflow
- Aim for 90%+ code coverage in business logic
- Test behavior, not implementation details

### Test Structure
- Use AAA pattern (Arrange-Act-Assert)
- Name tests as `[MethodName]_[Scenario]_[ExpectedResult]`
- Create separate test classes for each class being tested

### Example Test:
```csharp
[Fact]
public async Task GetItemsByLocation_WithinRadius_ReturnsMatchingItems()
{
    // Arrange
    var mockItems = new List<Item>
    {
        new Item { Latitude = 47.6062, Longitude = -122.3321 }, // Seattle
        new Item { Latitude = 37.7749, Longitude = -122.4194 }  // San Francisco
    };
    
    var mockService = new Mock<IItemService>();
    mockService.Setup(s => s.GetItemsAsync()).ReturnsAsync(mockItems);
    
    var viewModel = new LocationViewModel(mockService.Object);
    
    // Act
    var result = await viewModel.GetItemsByLocationAsync(47.6062, -122.3321, 10);
    
    // Assert
    Assert.Single(result);
    Assert.Equal(47.6062, result.First().Latitude);
}
```

## Technical Requirements

### Required Dependencies
- Use **CommunityToolkit.Mvvm** for MVVM implementation
- Use **SQLite-net-pcl** for local database
- Use **Azure.Identity** for authentication
- Use **Microsoft.Extensions.Http** for API calls
- Use **Xamarin.CommunityToolkit** for UI controls
- Use **xUnit** and **Moq** for testing

### Navigation
- Use MAUI Shell navigation exclusively
- Register routes in `AppShell.xaml.cs`
- Navigate via `Shell.Current.GoToAsync()`

### Data Persistence
- Use SQLite for local storage
- Create repositories for each model type
- Implement interfaces for all data access

### Authentication
- Use Microsoft Identity Platform
- Store tokens securely using `SecureStorage`
- Implement token refresh logic

### UI Guidelines
- Support both light and dark themes
- Design for both portrait and landscape orientations
- Ensure accessibility compliance
- Implement responsive layouts

## Do's and Don'ts

### Do
- ✅ Use dependency injection for all services
- ✅ Implement proper error handling with user-friendly messages
- ✅ Make network calls asynchronous
- ✅ Cache data where appropriate
- ✅ Write unit tests for all business logic

### Don't
- ❌ Put business logic in code-behind files
- ❌ Create static service instances
- ❌ Use Thread.Sleep (use Task.Delay instead)
- ❌ Hard-code connection strings or API keys
- ❌ Skip writing tests

## Feature-Specific Guidelines

### Marketplace Listings
- Implement virtual listing for large datasets
- Cache images locally
- Support offline viewing of previously loaded listings

### Location Services
- Handle location permission requests properly
- Use background location sparingly to preserve battery
- Implement distance calculations using the haversine formula

### Messaging
- Use Azure SignalR for real-time chat
- Store message history in SQLite
- Implement retry logic for failed message sends

### UI Components
- Create reusable custom controls in the Controls folder
- Use XAML styles for consistent appearance
- Follow Material Design guidelines
