using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
	public class TodoList
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		public HashSet<TodoItem> Items { get; set; } = new HashSet<TodoItem>();
	}
}
