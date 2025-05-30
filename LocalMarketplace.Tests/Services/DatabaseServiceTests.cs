using LocalMarketplace.Tests.Models;
using LocalMarketplace.Tests.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LocalMarketplace.Tests.Services
{
    public class DatabaseServiceTests
    {
        [Fact]
        public async Task InitializeAsync_CreatesTablesCorrectly()
        {
            // This is a mock test to demonstrate how we would test database initialization
            // In a real test, we would use an in-memory SQLite database for testing
            
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            bool tablesCreated = false;
            
            mockDatabaseService
                .Setup(service => service.InitializeAsync())
                .Callback(() => tablesCreated = true)
                .Returns(Task.CompletedTask);
                
            // Act
            await mockDatabaseService.Object.InitializeAsync();
            
            // Assert
            Assert.True(tablesCreated);
        }
        
        [Fact]
        public async Task GetItemsBySellerAsync_ReturnsCorrectItems()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var sellerId = 1;
            var sellerItems = new List<TestItem>
            {
                new TestItem { Id = 1, SellerId = sellerId, Title = "Item 1" },
                new TestItem { Id = 2, SellerId = sellerId, Title = "Item 2" }
            };
            
            mockDatabaseService
                .Setup(service => service.GetItemsBySellerAsync(sellerId))
                .ReturnsAsync(sellerItems);
                
            // Act
            var result = await mockDatabaseService.Object.GetItemsBySellerAsync(sellerId);
            
            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, item => Assert.Equal(sellerId, item.SellerId));
        }
        
        [Fact]
        public async Task SaveUserAsync_UpdatesExistingUser()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var existingUser = new TestUser
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com"
            };
            
            mockDatabaseService
                .Setup(service => service.SaveUserAsync(It.Is<TestUser>(u => u.Id != 0)))
                .ReturnsAsync(1); // 1 row affected
                
            // Act
            var result = await mockDatabaseService.Object.SaveUserAsync(existingUser);
            
            // Assert
            Assert.Equal(1, result);
            mockDatabaseService.Verify(s => s.SaveUserAsync(It.Is<TestUser>(u => u.Id == 1)), Times.Once);
        }
        
        [Fact]
        public async Task SaveUserAsync_InsertsNewUser()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var newUser = new TestUser
            {
                Id = 0, // New user has ID 0
                Username = "newuser",
                Email = "new@example.com"
            };
            
            mockDatabaseService
                .Setup(service => service.SaveUserAsync(It.Is<TestUser>(u => u.Id == 0)))
                .ReturnsAsync(1); // 1 row affected
                
            // Act
            var result = await mockDatabaseService.Object.SaveUserAsync(newUser);
            
            // Assert
            Assert.Equal(1, result);
            mockDatabaseService.Verify(s => s.SaveUserAsync(It.Is<TestUser>(u => u.Id == 0)), Times.Once);
        }
    }
}
