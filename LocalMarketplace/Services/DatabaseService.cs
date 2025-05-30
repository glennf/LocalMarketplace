using LocalMarketplace.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LocalMarketplace.Services
{
    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection? _database;

        public DatabaseService()
        {
            // Initialize in InitializeAsync method
        }

        // Private helper method to ensure the database is initialized
        private void EnsureDatabaseInitialized()
        {
            if (_database == null)
                throw new InvalidOperationException("Database not initialized. Call InitializeAsync first.");
        }

        public async Task InitializeAsync()
        {
            if (_database != null)
                return;

            // Get the database path
            string databasePath = Path.Combine(FileSystem.AppDataDirectory, "localmarketplace.db");

            // Create the connection
            _database = new SQLiteAsyncConnection(databasePath);

            // Create tables
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Item>();
            await _database.CreateTableAsync<Message>();

            // Seed initial data for testing if needed
            await SeedDataAsync();
        }

        private async Task SeedDataAsync()
        {
            EnsureDatabaseInitialized();
                
            // Check if we already have users
            var userCount = await _database!.Table<User>().CountAsync();
            if (userCount == 0)
            {
                // Add some test users
                var users = new List<User>
                {
                    new User
                    {
                        Username = "johndoe",
                        Email = "john@example.com",
                        PasswordHash = "hashedpassword1", // In a real app, use proper password hashing
                        PhoneNumber = "555-123-4567",
                        AverageRating = 4.5,
                        CreatedDate = DateTime.Now.AddDays(-30)
                    },
                    new User
                    {
                        Username = "janedoe",
                        Email = "jane@example.com",
                        PasswordHash = "hashedpassword2",
                        PhoneNumber = "555-987-6543",
                        AverageRating = 4.8,
                        CreatedDate = DateTime.Now.AddDays(-15)
                    }
                };

                foreach (var user in users)
                {
                    await _database!.InsertAsync(user);
                }
            }

            // Check if we already have items
            var itemCount = await _database!.Table<Item>().CountAsync();
            if (itemCount == 0)
            {
                // Add some test items
                var items = new List<Item>
                {
                    new Item
                    {
                        SellerId = 1,
                        Title = "Mountain Bike",
                        Description = "Lightly used mountain bike in great condition. Perfect for trails and city riding.",
                        Price = 250.00m,
                        Category = "Sports & Outdoors",
                        Condition = "Used - Good",
                        ImageUrls = new List<string> { "https://example.com/bike1.jpg" },
                        Latitude = 37.7749,
                        Longitude = -122.4194,
                        Location = "San Francisco, CA",
                        ListingDate = DateTime.Now.AddDays(-5),
                        IsActive = true
                    },
                    new Item
                    {
                        SellerId = 2,
                        Title = "iPhone 14 Pro",
                        Description = "iPhone 14 Pro in excellent condition. Includes charger and original box.",
                        Price = 899.99m,
                        Category = "Electronics",
                        Condition = "Used - Excellent",
                        ImageUrls = new List<string> { "https://example.com/iphone1.jpg", "https://example.com/iphone2.jpg" },
                        Latitude = 37.3382,
                        Longitude = -121.8863,
                        Location = "San Jose, CA",
                        ListingDate = DateTime.Now.AddDays(-2),
                        IsActive = true
                    }
                };

                foreach (var item in items)
                {
                    await _database!.InsertAsync(item);
                }
            }
        }

        // User methods
        public Task<List<User>> GetUsersAsync()
        {
            EnsureDatabaseInitialized();
            return _database!.Table<User>().ToListAsync();
        }

        public Task<User> GetUserAsync(int id)
        {
            EnsureDatabaseInitialized();
            return _database!.Table<User>()
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            EnsureDatabaseInitialized();
            return _database!.Table<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveUserAsync(User user)
        {
            EnsureDatabaseInitialized();
            if (user.Id != 0)
                return _database!.UpdateAsync(user);
            else
                return _database!.InsertAsync(user);
        }

        public Task<int> DeleteUserAsync(User user)
        {
            EnsureDatabaseInitialized();
            return _database!.DeleteAsync(user);
        }

        // Item methods
        public Task<List<Item>> GetItemsAsync()
        {
            EnsureDatabaseInitialized();
            return _database!.Table<Item>()
                .Where(i => i.IsActive)
                .OrderByDescending(i => i.ListingDate)
                .ToListAsync();
        }

        public Task<List<Item>> GetItemsByCategoryAsync(string category)
        {
            EnsureDatabaseInitialized();
            return _database!.Table<Item>()
                .Where(i => i.IsActive && i.Category == category)
                .OrderByDescending(i => i.ListingDate)
                .ToListAsync();
        }

        public Task<List<Item>> GetItemsBySellerAsync(int sellerId)
        {
            EnsureDatabaseInitialized();
            return _database!.Table<Item>()
                .Where(i => i.SellerId == sellerId)
                .OrderByDescending(i => i.ListingDate)
                .ToListAsync();
        }

        public Task<Item> GetItemAsync(int id)
        {
            EnsureDatabaseInitialized();
            return _database!.Table<Item>()
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(Item item)
        {
            EnsureDatabaseInitialized();
            if (item.Id != 0)
                return _database!.UpdateAsync(item);
            else
            {
                item.ListingDate = DateTime.Now;
                return _database!.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(Item item)
        {
            EnsureDatabaseInitialized();
            return _database!.DeleteAsync(item);
        }

        // Message methods
        public Task<List<Message>> GetMessagesAsync(int userId1, int userId2)
        {
            EnsureDatabaseInitialized();
            return _database!.Table<Message>()
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) || 
                            (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentDate)
                .ToListAsync();
        }

        public Task<List<Message>> GetMessagesByItemAsync(int itemId)
        {
            EnsureDatabaseInitialized();
            return _database!.Table<Message>()
                .Where(m => m.ItemId == itemId)
                .OrderBy(m => m.SentDate)
                .ToListAsync();
        }

        public Task<Message> GetMessageAsync(int id)
        {
            EnsureDatabaseInitialized();
            return _database!.Table<Message>()
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveMessageAsync(Message message)
        {
            EnsureDatabaseInitialized();
            if (message.Id != 0)
                return _database!.UpdateAsync(message);
            else
            {
                message.SentDate = DateTime.Now;
                message.IsRead = false;
                return _database!.InsertAsync(message);
            }
        }

        public Task<int> DeleteMessageAsync(Message message)
        {
            EnsureDatabaseInitialized();
            return _database!.DeleteAsync(message);
        }
    }
}
