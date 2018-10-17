using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	public class ListController : Controller
	{
		private readonly TodoService _todoService;

		public ListController(TodoService todoService)
		{
			_todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
		}

		/// <summary>
		/// Gets all available Todo lists
		/// </summary>
		/// <returns>Todo lists with their Ids and names</returns>
		[HttpGet]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User not authorized")]
		[SwaggerResponse(200, typeof(Dictionary<int, string>))]
		public IActionResult Get()
		{
			return Ok(_todoService.GetAllLists());
		}

		/// <summary>
		/// Gets the name of a specific list
		/// </summary>
		/// <param name="listId">Id of the list to fetch the name from</param>
		/// <returns>The name of the list</returns>
		[HttpGet("{listId}")]
		[SwaggerResponse(200, typeof(ValueViewModel))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User not authorized")]
		public IActionResult Get(int listId)
		{
			return Ok(new ValueViewModel() { Value = _todoService.GetListName(listId)});
		}

		[HttpPost]
		public IActionResult Post([FromBody] ValueViewModel data)
		{
			return Ok(new IdViewModel()
			{
				Id = _todoService.AddList(data.Value),
			});
		}

		[HttpPut("{listId}")]
		public IActionResult Put(int listId, [FromBody] ValueViewModel data)
		{
			_todoService.ChangeListName(listId, data.Value);
			return Ok();
		}
		
		[HttpDelete("{listId}")]
		public IActionResult Delete(int listId)
		{
			if (_todoService.DeleteList(listId))
				return Ok();

			return NotFound();
		}

	}
}