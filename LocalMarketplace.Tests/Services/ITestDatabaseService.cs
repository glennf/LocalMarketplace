using LocalMarketplace.Tests.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalMarketplace.Tests.Services
{
    public interface ITestDatabaseService
    {
        Task InitializeAsync();
        
        // User methods
        Task<List<TestUser>> GetUsersAsync();
        Task<TestUser> GetUserAsync(int id);
        Task<TestUser> GetUserByEmailAsync(string email);
        Task<int> SaveUserAsync(TestUser user);
        Task<int> DeleteUserAsync(TestUser user);
        
        // Item methods
        Task<List<TestItem>> GetItemsAsync();
        Task<List<TestItem>> GetItemsByCategoryAsync(string category);
        Task<List<TestItem>> GetItemsBySellerAsync(int sellerId);
        Task<TestItem> GetItemAsync(int id);
        Task<int> SaveItemAsync(TestItem item);
        Task<int> DeleteItemAsync(TestItem item);
        
        // Message methods
        Task<List<TestMessage>> GetMessagesAsync(int userId1, int userId2);
        Task<List<TestMessage>> GetMessagesByItemAsync(int itemId);
        Task<TestMessage> GetMessageAsync(int id);
        Task<int> SaveMessageAsync(TestMessage message);
        Task<int> DeleteMessageAsync(TestMessage message);
    }
}
