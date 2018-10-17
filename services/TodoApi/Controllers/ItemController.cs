using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
	[Authorize]
	[Route("api/list/{listId}/[controller]")]
	public class ItemController : Controller
	{
		private readonly TodoService _todoService;

		public ItemController(TodoService todoService)
		{
			_todoService = todoService;
		}

		// GET api/list/{listId}/item/
		[HttpGet]
		public IActionResult Get(int listId)
		{
			return Ok(_todoService.GetAllItems(listId));
		}

		// POST api/list/{listId}/item/
		[HttpPost()]
		public IActionResult Post(int listId, [FromBody] ValueViewModel data)
		{
			return Ok(new IdViewModel()
			{
				Id = _todoService.AddItem(listId, data.Value),
			});
		}

		// PUT api/list/{listId}/item/{itemId}
		[HttpPut("{itemId}")]
		public IActionResult Put(int listId, int itemId, [FromBody] ValueViewModel data)
		{
			_todoService.ChangeItemText(listId, itemId, data.Value);
			return Ok();
		}

		// POST api/list/{listId}/item/{itemId}/toggle
		[HttpPost("{itemId}/toggle")]
		public IActionResult Post(int listId, int itemId)
		{
			_todoService.ToggleItemDone(listId, itemId);
			return Ok();
		}

		// DELETE api/list/{listId}/item/{itemId}
		[HttpDelete("{itemId}")]
		public IActionResult Delete(int listId, int itemId)
		{
			if (_todoService.DeleteItem(listId, itemId))
				return Ok();

			return NotFound();
		}
	}
}
