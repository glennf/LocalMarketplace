using CommunityToolkit.Mvvm.Input;
using LocalMarketplace.Tests.Models;
using LocalMarketplace.Tests.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LocalMarketplace.Tests.ViewModels
{
    public class UserViewModelTests
    {
        [Fact]
        public async Task LoginCommand_WithValidCredentials_AuthenticatesUser()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var email = "test@example.com";
            var password = "password123";
            var passwordHash = "hashedpassword"; // In a real app, we'd hash the password
            
            var user = new TestUser
            {
                Id = 1,
                Email = email,
                PasswordHash = passwordHash,
                Username = "testuser"
            };
            
            mockDatabaseService
                .Setup(service => service.GetUserByEmailAsync(email))
                .ReturnsAsync(user);
                
            var viewModel = new TestUserViewModel(mockDatabaseService.Object)
            {
                Email = email,
                Password = password
            };
            
            // Act
            await viewModel.LoginCommand.ExecuteAsync(null);
            
            // Assert
            Assert.True(viewModel.IsAuthenticated);
            Assert.Equal(user.Id, viewModel.CurrentUser.Id);
            Assert.Equal(user.Username, viewModel.CurrentUser.Username);
        }
        
        [Fact]
        public async Task LoginCommand_WithInvalidCredentials_ShowsError()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var email = "wrong@example.com";
            
            mockDatabaseService
                .Setup(service => service.GetUserByEmailAsync(email))
                .ReturnsAsync((TestUser)null); // User not found
                
            var viewModel = new TestUserViewModel(mockDatabaseService.Object)
            {
                Email = email,
                Password = "wrongpassword"
            };
            
            // Act
            await viewModel.LoginCommand.ExecuteAsync(null);
            
            // Assert
            Assert.False(viewModel.IsAuthenticated);
            Assert.Null(viewModel.CurrentUser);
            Assert.True(viewModel.HasError);
            Assert.False(string.IsNullOrEmpty(viewModel.ErrorMessage));
        }
        
        [Fact]
        public async Task RegisterCommand_WithNewUser_CreatesAccount()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var email = "newuser@example.com";
            var username = "newuser";
            var password = "newpassword123";
            
            TestUser savedUser = null;
            
            // Setup - no existing user with this email
            mockDatabaseService
                .Setup(service => service.GetUserByEmailAsync(email))
                .ReturnsAsync((TestUser)null);
                
            // Setup - saving user
            mockDatabaseService
                .Setup(service => service.SaveUserAsync(It.IsAny<TestUser>()))
                .Callback<TestUser>(user => 
                {
                    user.Id = 1; // Simulate database assignment of ID
                    savedUser = user;
                })
                .ReturnsAsync(1);
                
            var viewModel = new TestUserViewModel(mockDatabaseService.Object)
            {
                Email = email,
                Username = username,
                Password = password
            };
            
            // Act
            await viewModel.RegisterCommand.ExecuteAsync(null);
            
            // Assert
            Assert.NotNull(savedUser);
            Assert.Equal(email, savedUser.Email);
            Assert.Equal(username, savedUser.Username);
            Assert.True(viewModel.IsAuthenticated);
            Assert.Equal(1, viewModel.CurrentUser.Id);
        }
        
        [Fact]
        public async Task RegisterCommand_WithExistingEmail_ShowsError()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var email = "existing@example.com";
            var existingUser = new TestUser
            {
                Id = 1,
                Email = email,
                Username = "existinguser"
            };
            
            // Setup - existing user with this email
            mockDatabaseService
                .Setup(service => service.GetUserByEmailAsync(email))
                .ReturnsAsync(existingUser);
                
            var viewModel = new TestUserViewModel(mockDatabaseService.Object)
            {
                Email = email,
                Username = "newusername",
                Password = "password123"
            };
            
            // Act
            await viewModel.RegisterCommand.ExecuteAsync(null);
            
            // Assert
            Assert.False(viewModel.IsAuthenticated);
            Assert.Null(viewModel.CurrentUser);
            Assert.True(viewModel.HasError);
            Assert.Contains("already exists", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }
        
        [Fact]
        public void LogoutCommand_WhenExecuted_ClearsCurrentUser()
        {
            // Arrange
            var mockDatabaseService = new Mock<ITestDatabaseService>();
            var viewModel = new TestUserViewModel(mockDatabaseService.Object);
            
            // Set up authenticated state
            viewModel.CurrentUser = new TestUser
            {
                Id = 1,
                Username = "testuser"
            };
            viewModel.IsAuthenticated = true;
            
            // Act
            viewModel.LogoutCommand.Execute(null);
            
            // Assert
            Assert.False(viewModel.IsAuthenticated);
            Assert.Null(viewModel.CurrentUser);
        }
    }
    
    // Test class that mimics the behavior of our future UserViewModel
    public class TestUserViewModel
    {
        private readonly ITestDatabaseService _databaseService;
        
        public TestUserViewModel(ITestDatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
            RegisterCommand = new AsyncRelayCommand(RegisterAsync, CanRegister);
            LogoutCommand = new RelayCommand(Logout);
        }
        
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAuthenticated { get; set; }
        public TestUser CurrentUser { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        
        public IAsyncRelayCommand LoginCommand { get; }
        public IAsyncRelayCommand RegisterCommand { get; }
        public IRelayCommand LogoutCommand { get; }
        
        private bool CanLogin() => !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
        
        private bool CanRegister() => !string.IsNullOrEmpty(Email) && 
                                      !string.IsNullOrEmpty(Username) && 
                                      !string.IsNullOrEmpty(Password);
        
        private async Task LoginAsync()
        {
            HasError = false;
            ErrorMessage = string.Empty;
            
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
                
                // In a real implementation, we would properly check the password hash
                // For this test, we'll just simulate successful authentication
                
                CurrentUser = user;
                IsAuthenticated = true;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error during login: {ex.Message}";
                IsAuthenticated = false;
                CurrentUser = null;
            }
        }
        
        private async Task RegisterAsync()
        {
            HasError = false;
            ErrorMessage = string.Empty;
            
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
                var newUser = new TestUser
                {
                    Email = Email,
                    Username = Username,
                    PasswordHash = Password, // In a real app, we would hash the password
                    CreatedDate = DateTime.Now
                };
                
                await _databaseService.SaveUserAsync(newUser);
                
                // Auto-login after registration
                CurrentUser = newUser;
                IsAuthenticated = true;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error during registration: {ex.Message}";
            }
        }
        
        private void Logout()
        {
            CurrentUser = null;
            IsAuthenticated = false;
        }
    }
}
