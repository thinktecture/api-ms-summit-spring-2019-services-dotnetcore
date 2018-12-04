using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    /// <summary>
    /// Handles requests relating to managing todo lists
    /// </summary>
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
        [SwaggerResponse(200, Type = typeof(Dictionary<int, string>))]
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
        [SwaggerResponse(200, Type = typeof(ValueViewModel))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User not authorized")]
        public IActionResult Get(int listId)
        {
            return Ok(new ValueViewModel() { Value = _todoService.GetListName(listId) });
        }

        /// <summary>
        /// Adds a new todo list
        /// </summary>
        /// <param name="data">A DTO that carries the name of the new todo list</param>
        /// <returns>The new id of the created todo list</returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(IdViewModel))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User not authorized")]
        public IActionResult Post([FromBody] ValueViewModel data)
        {
            return Ok(new IdViewModel()
            {
                Id = _todoService.AddList(data.Value),
            });
        }

        /// <summary>
        /// Updates an existing todo list
        /// </summary>
        /// <param name="listId">The Id of the list to change the name of</param>
        /// <param name="data">A DTO that carries the new name of the existing todo list</param>
        /// <returns>Whether changing the name was successful</returns>
        [HttpPut("{listId}")]
        [SwaggerResponse(200)]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User not authorized")]
        public IActionResult Put(int listId, [FromBody] ValueViewModel data)
        {
            _todoService.ChangeListName(listId, data.Value);
            return Ok();
        }

        /// <summary>
        /// Deletes an existing todo list
        /// </summary>
        /// <param name="listId">The Id of the list to delete</param>
        /// <returns>Whether deletion was successful</returns>
        [HttpDelete("{listId}")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404)]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User not authorized")]
        public IActionResult Delete(int listId)
        {
            if (_todoService.DeleteList(listId))
                return Ok();

            return NotFound();
        }
    }
}
