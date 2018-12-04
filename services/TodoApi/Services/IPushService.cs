using System.Threading.Tasks;

namespace TodoApi.Services
{
    public interface IPushService
    {
        Task SendItemAddedAsync(int listId, int itemId, string itemName);
        Task SendItemDeletedAsync(int listId, int itemId);
        Task SendItemDoneChangedAsync(int listId, int itemId, bool done);
        Task SendItemNameChangedAsync(int listId, int itemId, string newName);
        Task SendListCreatedAsync(int listId, string listName);
        Task SendListDeletedAsync(int listId);
        Task SendListRenamedAsync(int listId, string newName);
    }
}