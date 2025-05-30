using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalMarketplace.Models;
using LocalMarketplace.Services;
using System.Collections.ObjectModel;

namespace LocalMarketplace.ViewModels
{
    /// <summary>
    /// ViewModel for managing marketplace items
    /// </summary>
    public partial class ItemViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;

        /// <summary>
        /// Initializes a new instance of the ItemViewModel class
        /// </summary>
        /// <param name="databaseService">The database service to use for item operations</param>
        public ItemViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadItemsCommand = new AsyncRelayCommand(LoadItemsAsync);
            FilterItemsByCategoryCommand = new AsyncRelayCommand(FilterItemsByCategoryAsync);
            SaveItemCommand = new AsyncRelayCommand(SaveItemAsync, () => CanSave);
            DeleteItemCommand = new AsyncRelayCommand<Item>(DeleteItemAsync);
            NewItemCommand = new RelayCommand(CreateNewItem);
            
            Items = new ObservableCollection<Item>();
        }

        /// <summary>
        /// Collection of items loaded from the database
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Item> _items = new();

        /// <summary>
        /// Currently selected item for viewing or editing
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveItemCommand))]
        private Item _selectedItem;

        /// <summary>
        /// Category filter for items
        /// </summary>
        [ObservableProperty]
        private string _selectedCategory;

        /// <summary>
        /// Flag indicating if the view is currently loading data
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>
        /// Flag indicating if there was an error during loading
        /// </summary>
        [ObservableProperty]
        private bool _hasError;

        /// <summary>
        /// Error message to display
        /// </summary>
        [ObservableProperty]
        private string _errorMessage;

        /// <summary>
        /// Determines if the current item can be saved
        /// </summary>
        public bool CanSave => SelectedItem != null && 
                               !string.IsNullOrEmpty(SelectedItem.Title) &&
                               SelectedItem.Price > 0;

        /// <summary>
        /// Command to load all active items
        /// </summary>
        public IAsyncRelayCommand LoadItemsCommand { get; }

        /// <summary>
        /// Command to filter items by category
        /// </summary>
        public IAsyncRelayCommand FilterItemsByCategoryCommand { get; }

        /// <summary>
        /// Command to save the current item
        /// </summary>
        public IAsyncRelayCommand SaveItemCommand { get; }

        /// <summary>
        /// Command to delete an item
        /// </summary>
        public IAsyncRelayCommand<Item> DeleteItemCommand { get; }

        /// <summary>
        /// Command to create a new item
        /// </summary>
        public IRelayCommand NewItemCommand { get; }

        /// <summary>
        /// Loads all active items from the database
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task LoadItemsAsync()
        {
            try
            {
                IsLoading = true;
                HasError = false;
                
                var items = await _databaseService.GetItemsAsync();
                Items.Clear();
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error loading items: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Filters items by the selected category
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task FilterItemsByCategoryAsync()
        {
            try
            {
                IsLoading = true;
                HasError = false;
                
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
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error filtering items: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Saves the currently selected item
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task SaveItemAsync()
        {
            if (SelectedItem == null) return;
            
            try
            {
                IsLoading = true;
                HasError = false;
                
                await _databaseService.SaveItemAsync(SelectedItem);
                
                // Refresh the items list
                await LoadItemsAsync();
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error saving item: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Deletes the specified item
        /// </summary>
        /// <param name="item">The item to delete</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task DeleteItemAsync(Item item)
        {
            if (item == null) return;
            
            try
            {
                IsLoading = true;
                HasError = false;
                
                await _databaseService.DeleteItemAsync(item);
                
                // Remove from collection and refresh
                if (Items.Contains(item))
                {
                    Items.Remove(item);
                }
                
                // If we deleted the selected item, clear it
                if (SelectedItem == item)
                {
                    SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error deleting item: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Creates a new item and sets it as the selected item
        /// </summary>
        private void CreateNewItem()
        {
            SelectedItem = new Item
            {
                ListingDate = DateTime.Now,
                IsActive = true,
                ImageUrls = new List<string>()
            };
        }
    }
}
