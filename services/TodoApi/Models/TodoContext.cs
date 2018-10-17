using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
	public class TodoContext : DbContext
	{
		public DbSet<TodoList> Lists { get; set; }
		public DbSet<TodoItem> Items { get; set; }

		public TodoContext(DbContextOptions<TodoContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TodoList>().HasData(new
			{
				Id = 1,
				Name = "Initial list",
			});

			modelBuilder.Entity<TodoItem>().HasData(
				new
				{
					Id = 1,
					TodoListId = 1,
					Text = "First item",
					Done = false,
				},
				new
				{
					Id = 2,
					TodoListId = 1,
					Text = "Second item",
					Done = true,
				});

			base.OnModelCreating(modelBuilder);
		}
	}
}
