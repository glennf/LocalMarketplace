using LocalMarketplace.Tests.Models;
using System;
using Xunit;

namespace LocalMarketplace.Tests.Models
{
    public class UserTests
    {
        [Fact]
        public void User_Constructor_SetsDefaultValues()
        {
            // Arrange & Act
            var user = new TestUser();

            // Assert
            Assert.Equal(0, user.Id);
            Assert.Null(user.Username);
            Assert.Null(user.Email);
            Assert.Null(user.PasswordHash);
            Assert.Null(user.PhoneNumber);
            Assert.Null(user.ProfileImageUrl);
            Assert.Equal(0, user.AverageRating);
            Assert.Equal(default, user.CreatedDate);
        }

        [Fact]
        public void User_PropertyChanged_FiresWhenPropertyIsChanged()
        {
            // Arrange
            var user = new TestUser();
            bool propertyChangedFired = false;
            string propertyName = null;

            user.PropertyChanged += (sender, args) =>
            {
                propertyChangedFired = true;
                propertyName = args.PropertyName;
            };

            // Act
            user.Username = "testuser";

            // Assert
            Assert.True(propertyChangedFired);
            Assert.Equal(nameof(user.Username), propertyName);
        }

        [Fact]
        public void User_PropertyChanged_DoesNotFireWhenValueIsUnchanged()
        {
            // Arrange
            var user = new TestUser { Username = "testuser" };
            bool propertyChangedFired = false;

            user.PropertyChanged += (sender, args) =>
            {
                propertyChangedFired = true;
            };

            // Act
            user.Username = "testuser"; // Same value

            // Assert
            Assert.False(propertyChangedFired);
        }

        [Fact]
        public void User_EmailProperty_SetsAndGetsCorrectly()
        {
            // Arrange
            var user = new TestUser();
            string expectedEmail = "test@example.com";

            // Act
            user.Email = expectedEmail;

            // Assert
            Assert.Equal(expectedEmail, user.Email);
        }

        [Fact]
        public void User_AverageRatingProperty_SetsAndGetsCorrectly()
        {
            // Arrange
            var user = new TestUser();
            double expectedRating = 4.5;

            // Act
            user.AverageRating = expectedRating;

            // Assert
            Assert.Equal(expectedRating, user.AverageRating);
        }

        [Fact]
        public void User_CreatedDateProperty_SetsAndGetsCorrectly()
        {
            // Arrange
            var user = new TestUser();
            DateTime expectedDate = new DateTime(2025, 5, 30);

            // Act
            user.CreatedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, user.CreatedDate);
        }
    }
}
