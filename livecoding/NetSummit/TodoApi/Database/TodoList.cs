using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Database
{
	public class TodoList
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }

		public HashSet<TodoItem> Items { get; set; } = new HashSet<TodoItem>();
	}
}