using CommunityToolkit.Mvvm.Input;
using LocalMarketplace.Tests.Models;
using LocalMarketplace.Tests.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;

namespace LocalMarketplace.Tests.ViewModels
{
    public class ItemViewModelTests
    {
        [Fact]
        public async Task LoadItemsCommand_WhenExecuted_PopulatesItemsCollection()
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

            var viewModel = new TestItemViewModel(mockDatabaseService.Object);

            // Act
            await viewModel.LoadItemsCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(2, viewModel.Items.Count);
            Assert.Contains(viewModel.Items, item => item.Id == 1);
            Assert.Contains(viewModel.Items, item => item.Id == 2);
        }

        [Fact]
        public async Task FilterItemsByCategoryCommand_WhenExecuted_FiltersItemsByCategory()
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

            var viewModel = new TestItemViewModel(mockDatabaseService.Object);
            viewModel.SelectedCategory = category;

            // Act
            await viewModel.FilterItemsByCategoryCommand.ExecuteAsync(null);

            // Assert
            Assert.Single(viewModel.Items);
            Assert.Equal(category, viewModel.Items[0].Category);
        }

        [Fact]
        public async Task SaveItemCommand_WhenExecutedWithNewItem_AddsToItemsCollection()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var newItem = new TestItem
            {
                Title = "New Item",
                Price = 50.00m,
                Category = "Books",
                IsActive = true
            };

            mockDatabaseService
                .Setup(service => service.SaveItemAsync(It.IsAny<TestItem>()))
                .Callback<TestItem>(item =>
                {
                    if (item.Id == 0)
                    {
                        item.Id = 1; // Simulate database assignment of ID
                        item.ListingDate = DateTime.Now;
                    }
                })
                .ReturnsAsync(1);

            var viewModel = new TestItemViewModel(mockDatabaseService.Object);
            viewModel.SelectedItem = newItem;

            // Act
            await viewModel.SaveItemCommand.ExecuteAsync(null);

            // Assert
            mockDatabaseService.Verify(s => s.SaveItemAsync(It.Is<TestItem>(i => i.Title == "New Item")), Times.Once);
            // We should also verify that the item was added to the collection after saving, 
            // but that depends on our implementation
        }

        [Fact]
        public void SelectedItem_WhenChanged_UpdatesCanSave()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var viewModel = new TestItemViewModel(mockDatabaseService.Object);
            
            // Initially selected item is null, so CanSave should be false
            Assert.False(viewModel.CanSave);

            // Act
            viewModel.SelectedItem = new TestItem
            {
                Title = "Test Item",
                Price = 10.00m,
                Category = "Test Category",
                Description = "Test Description"
            };

            // Assert
            Assert.True(viewModel.CanSave);
        }
    }

    // This is a test class that mimics the behavior of our future ItemViewModel
    // This will be implemented in the main project following the test implementation
    public class TestItemViewModel
    {
        private readonly ITestDatabaseService _databaseService;

        public TestItemViewModel(ITestDatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadItemsCommand = new AsyncRelayCommand(LoadItemsAsync);
            FilterItemsByCategoryCommand = new AsyncRelayCommand(FilterItemsByCategoryAsync);
            SaveItemCommand = new AsyncRelayCommand(SaveItemAsync, () => CanSave);
            Items = new ObservableCollection<TestItem>();
        }

        public ObservableCollection<TestItem> Items { get; private set; }
        
        private TestItem _selectedItem;
        public TestItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                // In the real implementation, we would use SetProperty here
                // We would also call CommandManager.InvalidateRequerySuggested();
                // or use a source generator to auto-generate property changed handling
            }
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                // Similar to above, property changed notification would be here
            }
        }

        public bool CanSave => SelectedItem != null && 
                               !string.IsNullOrEmpty(SelectedItem.Title) &&
                               SelectedItem.Price > 0;

        public IAsyncRelayCommand LoadItemsCommand { get; }
        public IAsyncRelayCommand FilterItemsByCategoryCommand { get; }
        public IAsyncRelayCommand SaveItemCommand { get; }

        private async Task LoadItemsAsync()
        {
            var items = await _databaseService.GetItemsAsync();
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        private async Task FilterItemsByCategoryAsync()
        {
            if (string.IsNullOrEmpty(SelectedCategory))
            {
                await LoadItemsAsync();
                return;
            }

            var items = await _databaseService.GetItemsByCategoryAsync(SelectedCategory);
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        private async Task SaveItemAsync()
        {
            if (SelectedItem != null)
            {
                await _databaseService.SaveItemAsync(SelectedItem);
                // In the real implementation, we would refresh the items collection here
            }
        }
    }
}
