using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Database
{
	public class TodoItem
	{
		[Key]
		public int Id { get; set; }
		[Required, MaxLength(250)]
		public string Text { get; set; }
		public bool Done { get; set; }
		public int TodoListId { get; set; }

		public TodoList List { get; set; }
	}

	public class TodoDbContext : DbContext
	{
		public virtual DbSet<TodoList> Lists { get; set; }
		public virtual DbSet<TodoItem> Items { get; set; }

		public TodoDbContext(DbContextOptions<TodoDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TodoList>().HasData(
				new
				{
					Id = 1,
					Name = "Initial list"
				});

			modelBuilder.Entity<TodoItem>().HasData(
				new
				{
					Id = 1,
					TodoListId = 1,
					Text = "First Item",
					Done = false,
				},
				new
				{
					Id = 2,
					TodoListId = 1,
					Text = "Second Item",
					Done = true,
				}
				);

			base.OnModelCreating(modelBuilder);
		}
	}
}
