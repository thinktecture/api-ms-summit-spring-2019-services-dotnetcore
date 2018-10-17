using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PushApi.Hubs
{
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class ListHub : Hub
	{
		public async Task ListAdded(int listId, string listName)
		{
			await Clients.All.SendAsync("listAdded", listId, listName);
		}

		public async Task ListRenamed(int listId, string newName)
		{
			await Clients.All.SendAsync("listRenamed", listId, newName);
		}

		public async Task ListDeleted(int listId)
		{
			await Clients.All.SendAsync("listDeleted", listId);
		}

		public async Task ItemAdded(int listId, int itemId, string itemName)
		{
			await Clients.All.SendAsync("itemAdded", listId, itemId, itemName);
		}

		public async Task ItemNameChanged(int listId, int itemId, string newName)
		{
			await Clients.All.SendAsync("itemNameChanged", listId, itemId, newName);
		}

		public async Task ItemDoneChanged(int listId, int itemId, bool done)
		{
			await Clients.All.SendAsync("itemDoneChanged", listId, itemId, done);
		}

		public async Task ItemDeleted(int listId, int itemId)
		{
			await Clients.All.SendAsync("itemDeleted", listId, itemId);
		}
	}
}
