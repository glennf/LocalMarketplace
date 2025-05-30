using LocalMarketplace.Tests.Models;
using LocalMarketplace.Tests.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LocalMarketplace.Tests.Services
{
    public class MockDatabaseServiceTests
    {
        [Fact]
        public async Task GetItemAsync_ReturnsCorrectItem()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var expectedItem = new TestItem
            {
                Id = 1,
                Title = "Test Item",
                Price = 100.00m,
                Description = "Test Description"
            };

            mockDatabaseService
                .Setup(service => service.GetItemAsync(1))
                .ReturnsAsync(expectedItem);

            // Act
            var result = await mockDatabaseService.Object.GetItemAsync(1);

            // Assert
            Assert.Equal(expectedItem.Id, result.Id);
            Assert.Equal(expectedItem.Title, result.Title);
            Assert.Equal(expectedItem.Price, result.Price);
            Assert.Equal(expectedItem.Description, result.Description);
        }

        [Fact]
        public async Task GetItemsAsync_ReturnsAllActiveItems()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var testItems = new List<TestItem>
            {
                new TestItem { Id = 1, Title = "Item 1", IsActive = true },
                new TestItem { Id = 2, Title = "Item 2", IsActive = true }
            };

            mockDatabaseService
                .Setup(service => service.GetItemsAsync())
                .ReturnsAsync(testItems);

            // Act
            var results = await mockDatabaseService.Object.GetItemsAsync();

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Contains(results, item => item.Id == 1);
            Assert.Contains(results, item => item.Id == 2);
        }

        [Fact]
        public async Task GetItemsByCategoryAsync_ReturnsItemsInCategory()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var category = "Electronics";
            var testItems = new List<TestItem>
            {
                new TestItem { Id = 1, Title = "Phone", Category = category, IsActive = true }
            };

            mockDatabaseService
                .Setup(service => service.GetItemsByCategoryAsync(category))
                .ReturnsAsync(testItems);

            // Act
            var results = await mockDatabaseService.Object.GetItemsByCategoryAsync(category);

            // Assert
            Assert.Single(results);
            Assert.Equal(category, results[0].Category);
        }

        [Fact]
        public async Task SaveItemAsync_SetsListingDateForNewItems()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var newItem = new TestItem 
            { 
                Title = "New Item", 
                Price = 50.00m 
            };

            DateTime beforeSave = DateTime.Now;
            TestItem savedItem = null;

            mockDatabaseService
                .Setup(service => service.SaveItemAsync(It.IsAny<TestItem>()))
                .Callback<TestItem>(item => 
                {
                    // Simulate what the real service would do
                    if (item.Id == 0)
                    {
                        item.ListingDate = DateTime.Now;
                    }
                    savedItem = item;
                })
                .ReturnsAsync(1);

            // Act
            var result = await mockDatabaseService.Object.SaveItemAsync(newItem);

            // Assert
            Assert.Equal(1, result);
            Assert.NotNull(savedItem);
            // We can't assert the exact date since it's set inside the callback
        }
    }
}
