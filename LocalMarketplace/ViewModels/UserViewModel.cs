using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalMarketplace.Models;
using LocalMarketplace.Services;
using System;
using System.Threading.Tasks;

namespace LocalMarketplace.ViewModels
{
    /// <summary>
    /// ViewModel for user authentication and profile management
    /// </summary>
    public partial class UserViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        
        /// <summary>
        /// Initializes a new instance of the UserViewModel class
        /// </summary>
        /// <param name="databaseService">The database service to use for user operations</param>
        public UserViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
            RegisterCommand = new AsyncRelayCommand(RegisterAsync, CanRegister);
            LogoutCommand = new RelayCommand(Logout);
            UpdateProfileCommand = new AsyncRelayCommand(UpdateProfileAsync);
        }
        
        /// <summary>
        /// Email for login or registration
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        private string _email;
        
        /// <summary>
        /// Username for registration
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        private string _username;
        
        /// <summary>
        /// Password for login or registration
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        private string _password;
        
        /// <summary>
        /// Flag indicating if the user is authenticated
        /// </summary>
        [ObservableProperty]
        private bool _isAuthenticated;
        
        /// <summary>
        /// Currently logged in user
        /// </summary>
        [ObservableProperty]
        private User _currentUser;
        
        /// <summary>
        /// Flag indicating if there was an error during login/registration
        /// </summary>
        [ObservableProperty]
        private bool _hasError;
        
        /// <summary>
        /// Error message to display
        /// </summary>
        [ObservableProperty]
        private string _errorMessage;
        
        /// <summary>
        /// Flag indicating if the view is currently loading data
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;
        
        /// <summary>
        /// Command to log in a user
        /// </summary>
        public IAsyncRelayCommand LoginCommand { get; }
        
        /// <summary>
        /// Command to register a new user
        /// </summary>
        public IAsyncRelayCommand RegisterCommand { get; }
        
        /// <summary>
        /// Command to log out the current user
        /// </summary>
        public IRelayCommand LogoutCommand { get; }
        
        /// <summary>
        /// Command to update the current user's profile
        /// </summary>
        public IAsyncRelayCommand UpdateProfileCommand { get; }
        
        /// <summary>
        /// Determines if login can be executed
        /// </summary>
        /// <returns>True if login can be executed, otherwise false</returns>
        private bool CanLogin() => !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
        
        /// <summary>
        /// Determines if registration can be executed
        /// </summary>
        /// <returns>True if registration can be executed, otherwise false</returns>
        private bool CanRegister() => !string.IsNullOrEmpty(Email) && 
                                      !string.IsNullOrEmpty(Username) && 
                                      !string.IsNullOrEmpty(Password);
        
        /// <summary>
        /// Logs in a user with the provided credentials
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task LoginAsync()
        {
            HasError = false;
            ErrorMessage = string.Empty;
            IsLoading = true;
            
            try
            {
                var user = await _databaseService.GetUserByEmailAsync(Email);
                
                if (user == null)
                {
                    HasError = true;
                    ErrorMessage = "User not found. Please check your email and try again.";
                    IsAuthenticated = false;
                    CurrentUser = null;
                    return;
                }
                
                // In a real implementation, we would properly verify the password hash
                // For now, we'll just simulate successful authentication
                // In production, use a proper password hashing library like BCrypt
                
                CurrentUser = user;
                IsAuthenticated = true;
                
                // Clear sensitive data
                Password = string.Empty;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error during login: {ex.Message}";
                IsAuthenticated = false;
                CurrentUser = null;
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        /// <summary>
        /// Registers a new user with the provided information
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task RegisterAsync()
        {
            HasError = false;
            ErrorMessage = string.Empty;
            IsLoading = true;
            
            try
            {
                // Check if email already exists
                var existingUser = await _databaseService.GetUserByEmailAsync(Email);
                
                if (existingUser != null)
                {
                    HasError = true;
                    ErrorMessage = "A user with this email already exists.";
                    return;
                }
                
                // Create new user
                var newUser = new User
                {
                    Email = Email,
                    Username = Username,
                    PasswordHash = Password, // In a real app, we would hash the password
                    CreatedDate = DateTime.Now,
                    AverageRating = 0,
                    PhoneNumber = string.Empty // Will be set in profile update
                };
                
                await _databaseService.SaveUserAsync(newUser);
                
                // Auto-login after registration
                CurrentUser = newUser;
                IsAuthenticated = true;
                
                // Clear sensitive data
                Password = string.Empty;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error during registration: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        /// <summary>
        /// Logs out the current user
        /// </summary>
        private void Logout()
        {
            CurrentUser = null;
            IsAuthenticated = false;
            
            // Clear sensitive data
            Email = string.Empty;
            Password = string.Empty;
            Username = string.Empty;
        }
        
        /// <summary>
        /// Updates the current user's profile
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task UpdateProfileAsync()
        {
            if (CurrentUser == null) return;
            
            HasError = false;
            ErrorMessage = string.Empty;
            IsLoading = true;
            
            try
            {
                // Save the updated user profile
                await _databaseService.SaveUserAsync(CurrentUser);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error updating profile: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
