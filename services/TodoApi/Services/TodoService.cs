using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApi.Models;
#pragma warning disable 4014

namespace TodoApi.Services
{
	public class TodoService
	{
		private readonly ILogger<TodoService> _logger;
		private readonly TodoContext _todoContext;
		private readonly PushService _pushService;

		public TodoService(ILogger<TodoService> logger, TodoContext todoContext, PushService pushService)
		{
			_logger = logger;
			_todoContext = todoContext ?? throw new ArgumentNullException(nameof(todoContext));
			_pushService = pushService ?? throw new ArgumentNullException(nameof(pushService));
		}

		public IDictionary<int, string> GetAllLists()
		{
			_logger?.LogInformation("Retrieving all lists");

			return _todoContext
				.Lists
				.AsNoTracking()
				.OrderBy(l => l.Id)
				.ToDictionary(l => l.Id, l => l.Name);
		}
		
		public IEnumerable<ItemViewModel> GetAllItems(int listId)
		{
			return _todoContext
				.Items
				.AsNoTracking()
				.Where(i => i.TodoListId == listId)
				.OrderBy(l => l.Id)
				.Select(i => new ItemViewModel()
				{
					Id = i.Id,
					ListId = i.TodoListId,
					Text = i.Text,
					Done = i.Done,
				})
				.ToArray();
		}

		public int AddList(string name)
		{
			var existing = _todoContext.Lists
				.Select(l => new {l.Id, l.Name})
				.FirstOrDefault(l => l.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

			if (existing != null)
			{
				_logger?.LogInformation("Did not add list {ListName}, because list {ListId} did already exist.", name, existing.Id);
				return existing.Id;
			}

			var entity = new TodoList()
			{
				Name = name,
			};

			_logger.LogDebug("Adding list {ListName}", name);

			_todoContext.Add(entity);
			_todoContext.SaveChanges();

			_pushService.SendListCreated(entity.Id, entity.Name);

			return entity.Id;
		}
		
		public int AddItem(int listId, string text)
		{
			var existing = _todoContext.Items
				.Select(i => new { i.Id, i.Text })
				.FirstOrDefault(i => i.Text.Equals(text, StringComparison.InvariantCultureIgnoreCase));

			if (existing != null)
				return existing.Id;

			var entity = new TodoItem()
			{
				TodoListId = listId,
				Text = text,
			};

			_todoContext.Add(entity);
			_todoContext.SaveChanges();

			_pushService.SendItemAdded(listId, entity.Id, entity.Text);

			return entity.Id;
		}

		public void ChangeListName(int listId, string newName)
		{
			var entity = new TodoList()
			{
				Id = listId,
			};

			_todoContext.Attach(entity);
			entity.Name = newName;

			_todoContext.SaveChanges();

			_pushService.SendListRenamed(listId, newName);
		}
		
		public void ChangeItemText(int listId, int itemId, string newText)
		{
			var entity = new TodoItem()
			{
				Id = itemId,
			};

			_todoContext.Attach(entity);
			entity.Text = newText;

			_todoContext.SaveChanges();

			_pushService.SendItemNameChanged(listId, itemId, newText);
		}

		public void ToggleItemDone(int listId, int itemId)
		{
			var entity = _todoContext.Items
				.Find(itemId);

			entity.Done = !entity.Done;
			_logger?.LogInformation("Toggling list item {Item} with id {ItemId}, new state: {Done}", entity.Text, entity.Id, entity.Done);

			_todoContext.SaveChanges();
			_pushService.SendItemDoneChanged(listId, itemId, entity.Done);
		}

		public bool DeleteList(int listId)
		{
			var entity = new TodoList()
			{
				Id = listId,
			};

			_todoContext.Entry(entity).State = EntityState.Deleted;

			try
			{
				_todoContext.SaveChanges();
				_pushService.SendListDeleted(listId);
				return true;
			}
			catch (DbUpdateConcurrencyException)
			{
				return false;
			}
		}
		public bool DeleteItem(int listId, int itemId)
		{
			var entity = new TodoItem()
			{
				Id = itemId,
			};

			_todoContext.Entry(entity).State = EntityState.Deleted;

			try
			{
				_todoContext.SaveChanges();
				_pushService.SendItemDeleted(listId, itemId);
				return true;
			}
			catch (DbUpdateConcurrencyException)
			{
				return false;
			}
		}


		public string GetListName(int listId)
		{
			return _todoContext.Lists.Find(listId)?.Name;
		}

	}
}
