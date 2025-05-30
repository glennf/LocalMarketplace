using CommunityToolkit.Mvvm.Input;
using LocalMarketplace.Tests.Models;
using LocalMarketplace.Tests.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LocalMarketplace.Tests.ViewModels
{
    public class LocationViewModelTests
    {
        [Fact]
        public async Task GetItemsByLocation_WithinRadius_ReturnsMatchingItems()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var testItems = new List<TestItem>
            {
                new TestItem 
                { 
                    Id = 1, 
                    Title = "Seattle Item", 
                    Latitude = 47.6062, 
                    Longitude = -122.3321,  // Seattle
                    IsActive = true 
                },
                new TestItem 
                { 
                    Id = 2, 
                    Title = "San Francisco Item", 
                    Latitude = 37.7749, 
                    Longitude = -122.4194,  // San Francisco
                    IsActive = true 
                }
            };

            mockDatabaseService
                .Setup(service => service.GetItemsAsync())
                .ReturnsAsync(testItems);

            var viewModel = new TestLocationViewModel(mockDatabaseService.Object);
            await viewModel.LoadItemsCommand.ExecuteAsync(null);

            // Act - Search near Seattle with a small radius (10km)
            var result = await viewModel.GetItemsByLocationAsync(47.6062, -122.3321, 10);

            // Assert - Should only find the Seattle item
            Assert.Single(result);
            Assert.Equal("Seattle Item", result[0].Title);
        }

        [Fact]
        public async Task GetItemsByLocation_OutsideRadius_ReturnsNoItems()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var testItems = new List<TestItem>
            {
                new TestItem 
                { 
                    Id = 1, 
                    Title = "New York Item", 
                    Latitude = 40.7128, 
                    Longitude = -74.0060,  // New York
                    IsActive = true 
                }
            };

            mockDatabaseService
                .Setup(service => service.GetItemsAsync())
                .ReturnsAsync(testItems);

            var viewModel = new TestLocationViewModel(mockDatabaseService.Object);
            await viewModel.LoadItemsCommand.ExecuteAsync(null);

            // Act - Search near Seattle, New York is far away
            var result = await viewModel.GetItemsByLocationAsync(47.6062, -122.3321, 100);

            // Assert - Should find no items
            Assert.Empty(result);
        }

        [Fact]
        public void CalculateDistance_BetweenTwoPoints_ReturnsCorrectDistance()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var viewModel = new TestLocationViewModel(mockDatabaseService.Object);

            // Coordinates for New York and Los Angeles
            double lat1 = 40.7128; // New York
            double lon1 = -74.0060;
            double lat2 = 34.0522; // Los Angeles
            double lon2 = -118.2437;

            // Act
            double distance = viewModel.CalculateDistance(lat1, lon1, lat2, lon2);

            // Assert - The distance should be approximately 3935 km
            Assert.InRange(distance, 3900, 4000);
        }

        [Fact]
        public async Task UpdateUserLocation_SavesLocationToUser()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var user = new TestUser { Id = 1, Username = "testuser" };
            double latitude = 47.6062;
            double longitude = -122.3321;

            mockDatabaseService
                .Setup(service => service.SaveUserAsync(It.IsAny<TestUser>()))
                .ReturnsAsync(1);

            var viewModel = new TestLocationViewModel(mockDatabaseService.Object)
            {
                CurrentUser = user
            };

            // Act
            await viewModel.UpdateUserLocationAsync(latitude, longitude);

            // Assert
            Assert.Equal(latitude, user.Latitude);
            Assert.Equal(longitude, user.Longitude);
            mockDatabaseService.Verify(s => s.SaveUserAsync(user), Times.Once);
        }
    }

    // Test class that mimics the behavior of our future LocationViewModel
    public class TestLocationViewModel
    {
        private readonly ITestDatabaseService _databaseService;
        private List<TestItem> _loadedItems;

        public TestLocationViewModel(ITestDatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadItemsCommand = new AsyncRelayCommand(LoadItemsAsync);
            _loadedItems = new List<TestItem>();
        }

        public IAsyncRelayCommand LoadItemsCommand { get; }
        public TestUser CurrentUser { get; set; }

        private async Task LoadItemsAsync()
        {
            _loadedItems = await _databaseService.GetItemsAsync();
        }

        public async Task<List<TestItem>> GetItemsByLocationAsync(double latitude, double longitude, double radiusKm)
        {
            var result = new List<TestItem>();

            foreach (var item in _loadedItems)
            {
                double distance = CalculateDistance(latitude, longitude, item.Latitude, item.Longitude);
                if (distance <= radiusKm)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula for calculating distance between two points on Earth
            const double earthRadiusKm = 6371;

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusKm * c;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public async Task UpdateUserLocationAsync(double latitude, double longitude)
        {
            if (CurrentUser != null)
            {
                CurrentUser.Latitude = latitude;
                CurrentUser.Longitude = longitude;
                await _databaseService.SaveUserAsync(CurrentUser);
            }
        }
    }
}
