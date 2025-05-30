using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalMarketplace.Models;
using LocalMarketplace.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LocalMarketplace.ViewModels
{
    /// <summary>
    /// ViewModel for location-based operations and map features
    /// </summary>
    public partial class LocationViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        
        /// <summary>
        /// Initializes a new instance of the LocationViewModel class
        /// </summary>
        /// <param name="databaseService">The database service to use for data operations</param>
        public LocationViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadItemsCommand = new AsyncRelayCommand(LoadItemsAsync);
            SearchNearbyCommand = new AsyncRelayCommand(SearchNearbyAsync);
            UpdateUserLocationCommand = new AsyncRelayCommand<(double, double)>(
                coordinates => UpdateUserLocationAsync(coordinates.Item1, coordinates.Item2));
            
            NearbyItems = new ObservableCollection<Item>();
        }
        
        /// <summary>
        /// Collection of items near the current location
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Item> _nearbyItems = new();
        
        /// <summary>
        /// Flag indicating if the view is currently loading data
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;
        
        /// <summary>
        /// Current user for location updates
        /// </summary>
        [ObservableProperty]
        private User _currentUser;
        
        /// <summary>
        /// Current user latitude
        /// </summary>
        [ObservableProperty]
        private double _userLatitude;
        
        /// <summary>
        /// Current user longitude
        /// </summary>
        [ObservableProperty]
        private double _userLongitude;
        
        /// <summary>
        /// Search radius in kilometers
        /// </summary>
        [ObservableProperty]
        private double _searchRadiusKm = 10;
        
        /// <summary>
        /// Flag indicating if there was an error
        /// </summary>
        [ObservableProperty]
        private bool _hasError;
        
        /// <summary>
        /// Error message to display
        /// </summary>
        [ObservableProperty]
        private string _errorMessage;
        
        /// <summary>
        /// Command to load all items
        /// </summary>
        public IAsyncRelayCommand LoadItemsCommand { get; }
        
        /// <summary>
        /// Command to search for items near the current location
        /// </summary>
        public IAsyncRelayCommand SearchNearbyCommand { get; }
        
        /// <summary>
        /// Command to update the user's location
        /// </summary>
        public IAsyncRelayCommand<(double, double)> UpdateUserLocationCommand { get; }
        
        private ObservableCollection<Item> _allItems = new();
        
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
                _allItems.Clear();
                
                foreach (var item in items)
                {
                    _allItems.Add(item);
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
        /// Searches for items near the current user location
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task SearchNearbyAsync()
        {
            try
            {
                IsLoading = true;
                HasError = false;
                
                // Make sure we have items loaded
                if (_allItems.Count == 0)
                {
                    await LoadItemsAsync();
                }
                
                var nearbyItems = await GetItemsByLocationAsync(UserLatitude, UserLongitude, SearchRadiusKm);
                
                NearbyItems.Clear();
                foreach (var item in nearbyItems)
                {
                    NearbyItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error searching for nearby items: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        /// <summary>
        /// Gets items within the specified radius of a location
        /// </summary>
        /// <param name="latitude">The latitude of the center point</param>
        /// <param name="longitude">The longitude of the center point</param>
        /// <param name="radiusKm">The search radius in kilometers</param>
        /// <returns>A collection of items within the specified radius</returns>
        public async Task<ObservableCollection<Item>> GetItemsByLocationAsync(
            double latitude, double longitude, double radiusKm)
        {
            var result = new ObservableCollection<Item>();
            
            // Make sure we have items loaded
            if (_allItems.Count == 0)
            {
                await LoadItemsAsync();
            }

            foreach (var item in _allItems)
            {
                double distance = CalculateDistance(latitude, longitude, item.Latitude, item.Longitude);
                if (distance <= radiusKm)
                {
                    result.Add(item);
                }
            }

            return result;
        }
        
        /// <summary>
        /// Calculates the distance between two points using the Haversine formula
        /// </summary>
        /// <param name="lat1">Latitude of first point</param>
        /// <param name="lon1">Longitude of first point</param>
        /// <param name="lat2">Latitude of second point</param>
        /// <param name="lon2">Longitude of second point</param>
        /// <returns>Distance in kilometers</returns>
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
        
        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="degrees">The angle in degrees</param>
        /// <returns>The angle in radians</returns>
        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
        
        /// <summary>
        /// Updates the current user's location
        /// </summary>
        /// <param name="latitude">The user's latitude</param>
        /// <param name="longitude">The user's longitude</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task UpdateUserLocationAsync(double latitude, double longitude)
        {
            try
            {
                IsLoading = true;
                HasError = false;
                
                UserLatitude = latitude;
                UserLongitude = longitude;
                
                if (CurrentUser != null)
                {
                    // If we have a current user, update their location
                    CurrentUser.Latitude = latitude;
                    CurrentUser.Longitude = longitude;
                    await _databaseService.SaveUserAsync(CurrentUser);
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error updating location: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
