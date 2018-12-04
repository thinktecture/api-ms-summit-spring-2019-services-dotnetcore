using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    /// <summary>
    /// Handles requests related to managing todo items
    /// </summary>
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
        /// <summary>
        /// Gets the items of a specified todo list
        /// </summary>
        /// <param name="listId">The Id of the list to retrieve the items from</param>
        /// <returns>The items of the requested todo list</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(ItemViewModel[]))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User not authorized")]
        public IActionResult Get(int listId)
        {
            return Ok(_todoService.GetAllItems(listId));
        }

        // POST api/list/{listId}/item/
        /// <summary>
        /// Creates a new todo item
        /// </summary>
        /// <param name="listId">The id of the list to add this new item to</param>
        /// <param name="data">The name of the todo item to add to the specified list</param>
        /// <returns>The Id of the new created todo item</returns>
        [HttpPost()]
        [SwaggerResponse(200, Type = typeof(IdViewModel))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User not authorized")]
        public IActionResult Post(int listId, [FromBody] ValueViewModel data)
        {
            return Ok(new IdViewModel()
            {
                Id = _todoService.AddItem(listId, data.Value),
            });
        }

        // PUT api/list/{listId}/item/{itemId}
        /// <summary>
        /// Changes the name of an existing todo item
        /// </summary>
        /// <param name="listId">The id of the list this item belongs to</param>
        /// <param name="itemId">The id of the item to change</param>
        /// <param name="data">The new name of the todo item</param>
        /// <returns>Whether changing the name was successful</returns>
        [HttpPut("{itemId}")]
        [SwaggerResponse(200)]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User not authorized")]
        public IActionResult Put(int listId, int itemId, [FromBody] ValueViewModel data)
        {
            _todoService.ChangeItemText(listId, itemId, data.Value);
            return Ok();
        }

        // POST api/list/{listId}/item/{itemId}/toggle
        /// <summary>
        /// Toggles the done state of an existing todo item
        /// </summary>
        /// <param name="listId">The id of the list this item belongs to</param>
        /// <param name="itemId">The id of the item to be toggled</param>
        /// <returns>Whether toggling was successful</returns>
        [HttpPost("{itemId}/toggle")]
        [SwaggerResponse(200)]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User not authorized")]
        public IActionResult Post(int listId, int itemId)
        {
            _todoService.ToggleItemDone(listId, itemId);
            return Ok();
        }

        // DELETE api/list/{listId}/item/{itemId}
        /// <summary>
        /// Deletes an existing item from a list
        /// </summary>
        /// <param name="listId">The id of the list to delete this item from</param>
        /// <param name="itemId">The id of the item to delete</param>
        /// <returns></returns>
        [HttpDelete("{itemId}")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404)]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User not authorized")]
        public IActionResult Delete(int listId, int itemId)
        {
            if (_todoService.DeleteItem(listId, itemId))
                return Ok();

            return NotFound();
        }
    }
}
