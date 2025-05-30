using LocalMarketplace.Models;

namespace LocalMarketplace.Services
{
    public interface IDatabaseService
    {
        Task InitializeAsync();
        
        // User methods
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<int> SaveUserAsync(User user);
        Task<int> DeleteUserAsync(User user);
        
        // Item methods
        Task<List<Item>> GetItemsAsync();
        Task<List<Item>> GetItemsByCategoryAsync(string category);
        Task<List<Item>> GetItemsBySellerAsync(int sellerId);
        Task<Item> GetItemAsync(int id);
        Task<int> SaveItemAsync(Item item);
        Task<int> DeleteItemAsync(Item item);
        
        // Message methods
        Task<List<Message>> GetMessagesAsync(int userId1, int userId2);
        Task<List<Message>> GetMessagesByItemAsync(int itemId);
        Task<Message> GetMessageAsync(int id);
        Task<int> SaveMessageAsync(Message message);
        Task<int> DeleteMessageAsync(Message message);
    }
}
