using LocalMarketplace.Tests.Models;
using System;
using Xunit;

namespace LocalMarketplace.Tests.Models
{
    public class MessageTests
    {
        [Fact]
        public void Message_Constructor_SetsDefaultValues()
        {
            // Arrange & Act
            var message = new TestMessage();

            // Assert
            Assert.Equal(0, message.Id);
            Assert.Equal(0, message.SenderId);
            Assert.Equal(0, message.ReceiverId);
            Assert.Null(message.ItemId);
            Assert.Null(message.Content);
            Assert.Equal(default, message.SentDate);
            Assert.False(message.IsRead);
            Assert.Null(message.AttachmentUrl);
        }

        [Fact]
        public void Message_PropertyChanged_FiresWhenPropertyIsChanged()
        {
            // Arrange
            var message = new TestMessage();
            bool propertyChangedFired = false;
            string propertyName = null;

            message.PropertyChanged += (sender, args) =>
            {
                propertyChangedFired = true;
                propertyName = args.PropertyName;
            };

            // Act
            message.Content = "Test message content";

            // Assert
            Assert.True(propertyChangedFired);
            Assert.Equal(nameof(message.Content), propertyName);
        }

        [Fact]
        public void Message_PropertyChanged_DoesNotFireWhenValueIsUnchanged()
        {
            // Arrange
            var message = new TestMessage { Content = "Test message" };
            bool propertyChangedFired = false;

            message.PropertyChanged += (sender, args) =>
            {
                propertyChangedFired = true;
            };

            // Act
            message.Content = "Test message"; // Same value

            // Assert
            Assert.False(propertyChangedFired);
        }

        [Fact]
        public void Message_SentDateProperty_SetsAndGetsCorrectly()
        {
            // Arrange
            var message = new TestMessage();
            DateTime expectedDate = new DateTime(2025, 5, 30, 12, 0, 0);

            // Act
            message.SentDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, message.SentDate);
        }

        [Fact]
        public void Message_IsReadProperty_SetsAndGetsCorrectly()
        {
            // Arrange
            var message = new TestMessage();

            // Act
            message.IsRead = true;

            // Assert
            Assert.True(message.IsRead);
        }

        [Fact]
        public void Message_ItemId_CanBeNull()
        {
            // Arrange
            var message = new TestMessage();

            // Act
            message.ItemId = null;

            // Assert
            Assert.Null(message.ItemId);
        }
    }
}
