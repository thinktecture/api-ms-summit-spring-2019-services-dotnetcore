using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
	public class TodoItem
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Text { get; set; }
		public bool Done { get; set; }

		public int TodoListId { get; set; }
		public TodoList List { get; set; }
	}
}
