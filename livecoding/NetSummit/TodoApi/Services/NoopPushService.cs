using System.Threading.Tasks;

namespace TodoApi.Services
{
	public class NoopPushService : IPushService
	{
		public Task SendItemAddedAsync(int listId, int itemId, string itemName)
		{
			return Task.CompletedTask;
		}

		public Task SendItemDeletedAsync(int listId, int itemId)
		{
			return Task.CompletedTask;
		}

		public Task SendItemDoneChangedAsync(int listId, int itemId, bool done)
		{
			return Task.CompletedTask;
		}

		public Task SendItemNameChangedAsync(int listId, int itemId, string newName)
		{
			return Task.CompletedTask;
		}

		public Task SendListCreatedAsync(int listId, string listName)
		{
			return Task.CompletedTask;
		}

		public Task SendListDeletedAsync(int listId)
		{
			return Task.CompletedTask;
		}

		public Task SendListRenamedAsync(int listId, string newName)
		{
			return Task.CompletedTask;
		}
	}
}