using LocalMarketplace.Tests.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace LocalMarketplace.Tests.Models
{
    public class ItemTests
    {
        [Fact]
        public void Item_Constructor_SetsDefaultValues()
        {
            // Arrange & Act
            var item = new TestItem();

            // Assert
            Assert.Equal(0, item.Id);
            Assert.Equal(0, item.SellerId);
            Assert.Equal(0m, item.Price);
            Assert.Equal(0, item.Latitude);
            Assert.Equal(0, item.Longitude);
            Assert.False(item.IsActive);
        }

        [Fact]
        public void Item_PropertyChanged_FiresWhenPropertyIsChanged()
        {
            // Arrange
            var item = new TestItem();
            bool propertyChangedFired = false;
            string propertyName = null;

            item.PropertyChanged += (sender, args) =>
            {
                propertyChangedFired = true;
                propertyName = args.PropertyName;
            };

            // Act
            item.Title = "Test Item";

            // Assert
            Assert.True(propertyChangedFired);
            Assert.Equal(nameof(item.Title), propertyName);
        }

        [Fact]
        public void Item_PropertyChanged_DoesNotFireWhenValueIsUnchanged()
        {
            // Arrange
            var item = new TestItem { Title = "Test Item" };
            bool propertyChangedFired = false;

            item.PropertyChanged += (sender, args) =>
            {
                propertyChangedFired = true;
            };

            // Act
            item.Title = "Test Item"; // Same value

            // Assert
            Assert.False(propertyChangedFired);
        }

        [Fact]
        public void Item_PriceProperty_SetsAndGetsCorrectly()
        {
            // Arrange
            var item = new TestItem();
            decimal expectedPrice = 99.99m;

            // Act
            item.Price = expectedPrice;

            // Assert
            Assert.Equal(expectedPrice, item.Price);
        }

        [Fact]
        public void Item_LocationProperties_SetAndGetCorrectly()
        {
            // Arrange
            var item = new TestItem();
            double expectedLatitude = 37.7749;
            double expectedLongitude = -122.4194;
            string expectedLocation = "San Francisco, CA";

            // Act
            item.Latitude = expectedLatitude;
            item.Longitude = expectedLongitude;
            item.Location = expectedLocation;

            // Assert
            Assert.Equal(expectedLatitude, item.Latitude);
            Assert.Equal(expectedLongitude, item.Longitude);
            Assert.Equal(expectedLocation, item.Location);
        }

        [Fact]
        public void Item_ImageUrls_SetsAndGetsCorrectly()
        {
            // Arrange
            var item = new TestItem();
            var expectedImageUrls = new List<string> { "url1.jpg", "url2.jpg" };

            // Act
            item.ImageUrls = expectedImageUrls;

            // Assert
            Assert.Equal(expectedImageUrls, item.ImageUrls);
            Assert.Equal(2, item.ImageUrls.Count);
        }
    }
}
